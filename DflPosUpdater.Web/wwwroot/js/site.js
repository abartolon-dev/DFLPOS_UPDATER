
(function (window, document, $) {
    'use strict';

    if (!$) {
        console.warn('jQuery no esta disponible. No se inicializaron los plugins de UI.');
        return;
    }

    const app = {
        init: function () {
            this.configureValidation();
            this.initDataTables();
            this.initICheck();
            this.initSweetAlerts();
            this.initConfirmations();
        },

        configureValidation: function () {
            if (!$.validator) {
                return;
            }

            $.validator.setDefaults({
                errorClass: 'text-danger',
                errorElement: 'span',
                highlight: function (element) {
                    const $element = $(element);
                    $element.addClass('is-invalid');
                    $element.closest('.form-check').addClass('is-invalid');
                },
                unhighlight: function (element) {
                    const $element = $(element);
                    $element.removeClass('is-invalid');
                    $element.closest('.form-check').removeClass('is-invalid');
                }
            });
        },

        initDataTables: function () {
            if (!$.fn.DataTable) {
                return;
            }

            $('.js-datatable').each(function () {
                if ($.fn.DataTable.isDataTable(this)) {
                    return;
                }

                var $table = $(this);
                var pageLength = parseInt($table.data('page-length'), 10) || 10;
                var ordering = $table.data('ordering') !== false;
                var searching = $table.data('searching') !== false;

                $table.DataTable({
                    pageLength: pageLength,
                    ordering: ordering,
                    searching: searching,
                    autoWidth: false,
                    responsive: false,

                    lengthMenu: [
                        [10, 25, 50, 100, -1],
                        [10, 25, 50, 100, 'Todos']
                    ],
                    buttons: [],
                    language: {
                        sProcessing: "Procesando...",
                        sLengthMenu: "Mostrar _MENU_ registros",
                        sZeroRecords: "No se encontraron resultados",
                        sEmptyTable: "Ningún dato disponible en esta tabla",
                        sInfo: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                        sInfoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                        sInfoFiltered: "(filtrado de un total de _MAX_ registros)",
                        sSearch: "Buscar:",
                        sLoadingRecords: "Cargando...",
                        oPaginate: {
                            sFirst: "Primero",
                            sLast: "Último",
                            sNext: "Siguiente",
                            sPrevious: "Anterior"
                        },
                        oAria: {
                            sSortAscending: ": Activar para ordenar la columna de manera ascendente",
                            sSortDescending: ": Activar para ordenar la columna de manera descendente"
                        }
                    },

                    dom: "<'row'<'col-sm-4'l><'col-sm-4 text-left'B><'col-sm-4'f>t<'col-sm-6'i><'col-sm-6'p>>",
                });
            });
        },

        initICheck: function () {
            if (!$.fn.iCheck) {
                return;
            }      
            const $inputs = $('input[type="checkbox"].form-check-input, input[type="radio"].form-check-input')
                .not('.icheck-ignore')
                .not('[data-icheck="false"]');

            $inputs.each(function () {
                const $input = $(this);

                if ($input.parent().hasClass('icheckbox_square-blue') || $input.parent().hasClass('iradio_square-blue')) {
                    return;
                }

                $input.iCheck({
                    checkboxClass: 'icheckbox_square-blue',
                    radioClass: 'iradio_square-blue',
                    increaseArea: '20%'
                });
            });

            $inputs.on('ifChanged', function () {
                const $input = $(this);
                $input.trigger('change');

                if ($.validator && $input.closest('form').data('validator')) {
                    $input.valid();
                }
            });
        },

        initSweetAlerts: function () {
            if (!window.Swal) {
                return;
            }

            const messages = window.DflPosUpdaterMessages || {};

            if (messages.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Correcto',
                    text: messages.success,
                    timer: 2600,
                    showConfirmButton: false
                });
            }

            if (messages.error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: messages.error
                });
            }
        },


        initConfirmations: function () {
            if (!window.Swal) {
                return;
            }

            $(document).on('submit', 'form[data-confirm="true"]', function (event) {
                const form = this;
                const $form = $(form);

                if ($form.data('confirmed') === true) {
                    return;
                }

                event.preventDefault();

                Swal.fire({
                    icon: $form.data('confirm-icon') || 'warning',
                    title: $form.data('confirm-title') || 'Confirmar accion',
                    text: $form.data('confirm-text') || 'Esta accion no se puede deshacer.',
                    showCancelButton: true,
                    confirmButtonText: $form.data('confirm-button') || 'Si, continuar',
                    cancelButtonText: $form.data('cancel-button') || 'Cancelar',
                    reverseButtons: true
                }).then(function (result) {
                    if (result.isConfirmed) {
                        $form.data('confirmed', true);
                        form.submit();
                    }
                });
            });
        }
    };

    $(function () {
        app.init();
    });

    window.DflPosUpdaterUi = app;
})(window, document, window.jQuery);
