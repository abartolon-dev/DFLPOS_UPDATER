(function (window, document, $) {
    'use strict';

    $(function () {
        const $tablaSucursales = $('#tblSucursales');

        if (!$tablaSucursales.length) {
            return;
        }

        if (!$.fn.DataTable) {
            console.warn('DataTables no esta disponible para inicializar #tblSucursales.');
            return;
        }

        if ($.fn.DataTable.isDataTable($tablaSucursales[0])) {
            return;
        }

        $tablaSucursales.DataTable({
            pageLength: 10,
            ordering: true,
            searching: true,
            autoWidth: false,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Todos']],
            columnDefs: [
                {
                    targets: -1,
                    orderable: false,
                    searchable: false
                }
            ],
            language: {
                processing: 'Procesando...',
                search: 'Buscar:',
                lengthMenu: 'Mostrar _MENU_ registros',
                info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
                infoEmpty: 'Mostrando 0 a 0 de 0 registros',
                infoFiltered: '(filtrado de _MAX_ registros totales)',
                loadingRecords: 'Cargando...',
                zeroRecords: 'No se encontraron resultados',
                emptyTable: 'No hay sucursales registradas',
                paginate: {
                    first: 'Primero',
                    previous: 'Anterior',
                    next: 'Siguiente',
                    last: 'Ultimo'
                },
                aria: {
                    sortAscending: ': activar para ordenar ascendente',
                    sortDescending: ': activar para ordenar descendente'
                }
            }
        });
    });
})(window, document, window.jQuery);
