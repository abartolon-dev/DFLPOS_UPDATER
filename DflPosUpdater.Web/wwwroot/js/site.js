/*
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

                const $table = $(this);
                const pageLength = parseInt($table.data('page-length'), 10) || 10;
                const ordering = $table.data('ordering') !== false;
                const searching = $table.data('searching') !== false;

                $table.DataTable({
                    pageLength: pageLength,
                    ordering: ordering,
                    searching: searching,
                    autoWidth: false,
                    lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Todos']],
                    language: {
                        processing: 'Procesando...',
                        search: 'Buscar:',
                        lengthMenu: 'Mostrar _MENU_ registros',
                        info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
                        infoEmpty: 'Mostrando 0 a 0 de 0 registros',
                        infoFiltered: '(filtrado de _MAX_ registros totales)',
                        infoPostFix: '',
                        loadingRecords: 'Cargando...',
                        zeroRecords: 'No se encontraron resultados',
                        emptyTable: 'No hay datos disponibles',
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
*/