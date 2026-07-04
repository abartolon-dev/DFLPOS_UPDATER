/*! RowsGroup for DataTables v1.0.0
 * 2024 Alex Hinojosa
 */

(function ($) {

	ShowedDataSelectorModifier = {
		order: 'current',
		page: 'current',
		search: 'applied',
	}

	GroupedColumnsOrderDir = 'asc';

	var RowsGroup = function (dt, columnsForGrouping) {
		this.table = dt.table();
		this.columnsForGrouping = columnsForGrouping;
		this.orderOverrideNow = false;
		this.order = []

		self = this;
		$(document).on('order.dt', function (e, settings) {
			if (!self.orderOverrideNow) {
				self._updateOrderAndDraw()
			}
			self.orderOverrideNow = false;
		})

		$(document).on('draw.dt', function (e, settings) {
			self._mergeCells()
		})

		this._updateOrderAndDraw();
	};

	RowsGroup.prototype = {
		_getOrderWithGroupColumns: function (order, groupedColumnsOrderDir) {
			if (groupedColumnsOrderDir === undefined)
				groupedColumnsOrderDir = GroupedColumnsOrderDir

			var self = this;
			var groupedColumnsIndexes = this.columnsForGrouping.map(function (columnSelector) {
				return self.table.column(columnSelector).index()
			})
			var groupedColumnsKnownOrder = order.filter(function (columnOrder) {
				return groupedColumnsIndexes.indexOf(columnOrder[0]) >= 0
			})
			var nongroupedColumnsOrder = order.filter(function (columnOrder) {
				return groupedColumnsIndexes.indexOf(columnOrder[0]) < 0
			})
			var groupedColumnsKnownOrderIndexes = groupedColumnsKnownOrder.map(function (columnOrder) {
				return columnOrder[0]
			})
			var groupedColumnsOrder = groupedColumnsIndexes.map(function (iColumn) {
				var iInOrderIndexes = groupedColumnsKnownOrderIndexes.indexOf(iColumn)
				if (iInOrderIndexes >= 0)
					return [iColumn, groupedColumnsKnownOrder[iInOrderIndexes][1]]
				else
					return [iColumn, groupedColumnsOrderDir]
			})

			groupedColumnsOrder.push.apply(groupedColumnsOrder, nongroupedColumnsOrder)
			return groupedColumnsOrder;
		},

		_getInjectedMonoSelectWorkaround: function (order) {
			if (order.length === 1) {
				// got mono order - workaround here
				var orderingColumn = order[0][0]
				var previousOrder = this.order.map(function (val) {
					return val[0]
				})
				var iColumn = previousOrder.indexOf(orderingColumn);
				if (iColumn >= 0) {
					return [[orderingColumn, this._toogleDirection(this.order[iColumn][1])]]
				}
			}
			return order;
		},

		_mergeCells: function () {
			var columnsIndexes = this.table.columns(this.columnsForGrouping, ShowedDataSelectorModifier).indexes().toArray()
			var showedRowsCount = this.table.rows(ShowedDataSelectorModifier)[0].length
			this._mergeColumn(0, showedRowsCount - 1, columnsIndexes)
		},

		_mergeColumn: function (iStartRow, iFinishRow, columnsIndexes) {
			var columnsIndexesCopy = columnsIndexes.slice()
			currentColumn = columnsIndexesCopy.shift()
			currentColumn = this.table.column(currentColumn, ShowedDataSelectorModifier)

			var columnNodes = currentColumn.nodes()
			var columnValues = currentColumn.data()

			var newSequenceRow = iStartRow,
				iRow;
			for (iRow = iStartRow + 1; iRow <= iFinishRow; ++iRow) {

				if (columnValues[iRow] === columnValues[newSequenceRow]) {
					$(columnNodes[iRow]).hide()
				} else {
					$(columnNodes[newSequenceRow]).show()
					$(columnNodes[newSequenceRow]).attr('rowspan', (iRow - 1) - newSequenceRow + 1)

					if (columnsIndexesCopy.length > 0)
						this._mergeColumn(newSequenceRow, (iRow - 1), columnsIndexesCopy)

					newSequenceRow = iRow;
				}

			}
			$(columnNodes[newSequenceRow]).show()
			$(columnNodes[newSequenceRow]).attr('rowspan', (iRow - 1) - newSequenceRow + 1)
			if (columnsIndexesCopy.length > 0)
				this._mergeColumn(newSequenceRow, (iRow - 1), columnsIndexesCopy)
		},

		_toogleDirection: function (dir) {
			return dir == 'asc' ? 'desc' : 'asc';
		},

		_updateOrderAndDraw: function () {
			this.orderOverrideNow = true;

			var currentOrder = this.table.order();
			currentOrder = this._getInjectedMonoSelectWorkaround(currentOrder);
			this.order = this._getOrderWithGroupColumns(currentOrder)
			this.table.order($.extend(true, Array(), this.order))
			this.table.draw()
		},
	};


	$.fn.dataTable.RowsGroup = RowsGroup;
	$.fn.DataTable.RowsGroup = RowsGroup;

	$(document).on('init.dt', function (e, settings) {
		if (e.namespace !== 'dt') {
			return;
		}

		var api = new $.fn.dataTable.Api(settings);

		if (settings.oInit.rowsGroup ||
			$.fn.dataTable.defaults.rowsGroup) {
			options = settings.oInit.rowsGroup ?
				settings.oInit.rowsGroup :
				$.fn.dataTable.defaults.rowsGroup;
			new RowsGroup(api, options);
		}
	});

}(jQuery));
