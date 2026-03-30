/*!
 * En este archivo podemos especificar configuraciones globales para toda la aplicación.
 * Tambien se define un objeto el cual se usa como namespace para acceder a variables y funciones globales del namespace.
 */


//Crear un objeto que sirva como NameSpace para acceder globalmente desde cualquier otro script...
var AppGlobal = {
    //Devuelve la url root de la app...
    RootUrl: (function () {
        let input = $("#root-url-input");
        let value = input.val();
        input.remove();
        return value;
    })(),
    //Devuelve la url del visor de reportes...
    ReportUrl: (function () {
        return "Reportes/ReporteCatalogo.aspx";
        // return "Reportes/ActivoFijoVistaPrevia.aspx";
    })(),
    ServerDate: (function () {
        let input = $("#server-date");
        let value = input.val();
        input.remove();
        return value;
    })(),

    //Funcion mostrar notificación.
    ShowNotification: function (options) {
        return $.notify({
            // options
            message: options.message,
            icon: options.icon
        },
            {
                // settings
                element: options.element,
                position: options.position,
                type: options.type,
                mouse_over: 'pause',
                z_index: 1100, //Sobreponer incluso a los modales de bootstrap (1050)...
                offset: {
                    x: 0,
                    y: 60
                },
                placement: {
                    from: "top",
                    align: "center"
                },
                animate: {
                    enter: options.animateEnter,
                    exit: options.animateExit
                }
            });
    },

    MessagesNotify: {
        AjaxRequestError: "¡Ha ocurrido un error procesando su petición, intente nuevamente y de persistir el problema contacte con el administrador del sistema.!",
        ShowNotifyCorrect: (message)=>{
            AppGlobal.ShowNotification({
                element: "body",
                message: message || "OK",
                type: "success",
                icon: "fas fa-check",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        },
        ShowNotifyError: (message) => {
            AppGlobal.ShowNotification({
                element: "body",
                message: message || AppGlobal.MessagesNotify.AjaxRequestError,
                type: "danger",
                icon: "fas fa-exclamation-triangle",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        },
        ShowNotifyWarning: (message) => {
            AppGlobal.ShowNotification({
                element: "body",
                message: message || AppGlobal.MessagesNotify.AjaxRequestError,
                type: "warning",
                icon: "fas fa-exclamation-triangle",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        }
    },
    //Funcion Deshabilitar teclado...
    DisableKeyboard: function (disabled) {
        if (disabled === true) {
            $(document).on("keydown", function (e) {
                e.preventDefault();
            });
        }
        else {
            $(document).off("keydown");
        }
    },
    //Funcion que parse validaciones jquery en contenido dinamico agregado al documento.
    ParseDynamicContent: function (selector) {
        if ($.validator) {
            $.validator.unobtrusive.parseDynamicContent(selector)
        }
    },
    //Funcion para obtener la fecha local del cliente en string en formarto dd/MM/yyyy.
    GetCurrentDateString: function () {
        let dateString = new Date().toLocaleString("es-NI", { day: "numeric", month: "2-digit", year: "numeric" });
        return dateString;
    },
    GetCurrentDateTimeString: function (horaValue) {
        let dateString = new Date().toLocaleString("es-NI", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit",
            hour12: false,
          }).replace(/[/]/g, "-").replace(",", "");

          var fechaYHora = dateString.split(' '); // Separamos la fecha de la hora

          var fecha = fechaYHora[0].split('-'); // Separamos la fecha que esta en formato DD-MM-YYYY

          var hora = fechaYHora[1]; // Guardamos la hora

          if(horaValue)
            hora = horaValue; // Formato hora: HH:MM

          var newFecha = `${fecha[2]}-${fecha[1]}-${fecha[0]} ${hora}`; // Unimos todas las partes en fromato YYYY-MM-DD HH:MM

        return newFecha;
    },
    //Devuelve un string de fecha con el formato especificado.
    FormatDate(dateString, format) {
        var dateValue = new Date(dateString);
        var day = dateValue.getDate();
        var month = dateValue.getMonth() + 1;
        var year = dateValue.getFullYear();
        var hour = dateValue.getHours();
        var minutes = dateValue.getMinutes();
        var seconds = dateValue.getSeconds();
        var dateToReturn = "";

        switch (format) {
            case "dd/mm/yyyy": {
                dateToReturn = day + "/" + month + "/" + year;
            } break;
            case "dd-mm-yyyy": {
                dateToReturn = day + "-" + month + "-" + year;
            } break;
            case "yyyy-MM-dd": {//padStart inserta al inicio el caracter 0 hasta alcanzar la longitud final de la cadena de 2
                dateToReturn = year + "-" + month.toString().padStart(2, '0') + "-" + day.toString().padStart(2, '0')
            } break;

            case "dd/mm/yyyy hh:mm:ss": {
                dateToReturn = day + "/" + month + "/" + year + " " + hour + ":" + minutes + ":" + seconds;
            } break;

            default:
                dateString = null;
                break;
        }

        return dateToReturn;
    },

    ConvertB64ToBlob: function (b64Data, contentType = '', sliceSize = 512) {
        const byteCharacters = atob(b64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    },

    //Mensajes del Sistema...
    Messages: {
        AjaxRequestError: "¡Ha ocurrido un error procesando su petición, intente nuevamente y de persistir el problema contacte con el administrador del sistema.!",
        ShowNotifyCorrect: (message)=>{
            showNotificationPreventDuplicates({
                element: "body",
                message: message,
                type: "success",
                icon: "fas fa-check",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        },
        ShowNotifyError: (message) => {
            showNotificationPreventDuplicates({
                element: "body",
                message: message || AppGlobal.Messages.AjaxRequestError,
                type: "danger",
                icon: "fas fa-exclamation-triangle",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        },
        ShowNotifyWarning: (message) => {
            showNotificationPreventDuplicates({
                element: "body",
                message: message || AppGlobal.Messages.AjaxRequestError,
                type: "warning",
                icon: "fas fa-exclamation-triangle",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            })
        }
    },
    validateMessage: function (typeMessage, Message, textButton, classButton) { Swal.fire({ icon: typeMessage, html: "<strong>" + Message + "</strong>", confirmButtonText: textButton, customClass: { confirmButton: classButton }, buttonsStyling: false, allowEscapeKey: false, allowOutsideClick: false, focusConfirm: true }); }
    ,
    startLoad: function () { $('body').addClass('ocultar_scroll'); $('div.overlay').show(); $("#frmLoadingGlobalState").removeClass('hideLoaderGIF').fadeIn('slow'); $('#foco').focus(); }
    ,
    endLoad: function () { $('div.overlay').fadeOut('slow'); $("#frmLoadingGlobalState").addClass('hideLoaderGIF').hide(); $('body').removeClass('ocultar_scroll'); },

    SmartwizardOptionsModal: function (data) { //Opciones de configuracion para el pluggin smartwizard...
        var option =
        {
            selected: 0, // Initial selected step, 0 = first step
            theme: data.theme || 'arrows', // theme for the wizard, related css need to include for other than default theme
            justified: true, // Nav menu justification. true/false
            autoAdjustHeight: true, // Automatically adjust content height
            backButtonSupport: true, // Enable the back button support
            enableUrlHash: data.enableUrlHash || true, // Enable selection of the step based on url hash
            transition: {
                animation: data.animation || 'none', // Animation effect on navigation, none|fade|slideHorizontal|slideVertical|slideSwing|css(Animation CSS class also need to specify)
                speed: '400', // Animation speed. Not used if animation is 'css'
                easing: '', // Animation easing. Not supported without a jQuery easing plugin. Not used if animation is 'css'
                prefixCss: '', // Only used if animation is 'css'. Animation CSS prefix
                fwdShowCss: '', // Only used if animation is 'css'. Step show Animation CSS on forward direction
                fwdHideCss: '', // Only used if animation is 'css'. Step hide Animation CSS on forward direction
                bckShowCss: '', // Only used if animation is 'css'. Step show Animation CSS on backward direction
                bckHideCss: '', // Only used if animation is 'css'. Step hide Animation CSS on backward direction
            },
            toolbar: {
                position: 'bottom', // none|top|bottom|both
                showNextButton: true, // show/hide a Next button
                showPreviousButton: true, // show/hide a Previous button
                extraHtml: `<!--ko if: $root.StepNumber() === ${data.StepSaveButton} -->
                    <button type="submit" id="btnSave" class="btn btn-success text-white" data-bind="enable: !($parent.ProcessingAction() /*|| $root.StepNumber() !== 3*/)">
                    <!--ko ifnot:$parent.ProcessingAction-->
                    <span class="fas fa-save" data-bind="css:$parent.SaveButtonIconClass"></span>
                    <span data-bind="text:$parent.SaveButtonText">Guardar</span>
                    <!--/ko-->
                    <!--ko if:$parent.ProcessingAction-->
                    <span class="fas fa-cog fa-spin"></span> Procesando...
                    <!--/ko-->
                    </button>
                    <!--/ko-->
                    ` // Extra html to show on toolbar
            },
            anchor: {
                enableNavigation: true, // Enable/Disable anchor navigation 
                enableNavigationAlways: false, // Activates all anchors clickable always
                enableDoneState: true, // Add done state on visited steps
                markPreviousStepsAsDone: true, // When a step selected by url hash, all previous steps are marked done
                unDoneOnBackNavigation: false, // While navigate back, done state will be cleared
                enableDoneStateNavigation: true // Enable/Disable the done state navigation
            },
            keyboard: {
                keyNavigation: true, // Enable/Disable keyboard navigation(left and right keys are used if enabled)
                keyLeft: [37], // Left key code
                keyRight: [39] // Right key code
            },
            lang: { // Language variables for button
                next: 'Siguiente',
                previous: 'Atras'
            },
            disabledSteps: data.disabledSteps || [], // Array Steps disabled
            errorSteps: [], // Array Steps error
            warningSteps: [], // Array Steps warning
            hiddenSteps: data.hiddenSteps || [], // Hidden steps
            getContent: null // Callback function for content loading
        }
        return option;
    },

    CalcularEdad: function CalcularEdad(fecha) {
        // Calculo la fecha de hoy
        hoy = new Date();

        // Si la fecha es indefinida, uso la fecha de hoy
        if (!fecha) {
            return 0;
        } else {
            // Calculo la fecha que recibo
            // La descompongo en un array
            var array_fecha = fecha.split("/");
            // Si el array no tiene tres partes, la fecha es incorrecta
            if (array_fecha.length != 3) {
                return false;
            }

            // Compruebo que los año, mes, día son correctos
            var ano = parseInt(array_fecha[2]);
            if (isNaN(ano)) {
                return false;
            }

            var mes = parseInt(array_fecha[1]);
            if (isNaN(mes)) {
                return false;
            }

            var dia = parseInt(array_fecha[0]);
            if (isNaN(dia)) {
                return false;
            }

            // Si el año de la fecha que recibo solo tiene 2 cifras, hay que cambiarlo a 4
            if (ano <= 99) {
                ano += 1900;
            }

            // Resto los años de las dos fechas
            edad = hoy.getFullYear() - ano - 1; // -1 porque no se si ha cumplido años ya este año

            // Si resto los meses y me da menor que 0, entonces no ha cumplido años.
            // Si da mayor, sí ha cumplido
            if (hoy.getMonth() + 1 - mes < 0) {
                return edad;
            }
            if (hoy.getMonth() + 1 - mes > 0) {
                return edad + 1;
            }

            // Entonces es que eran iguales. Miro los días.
            // Si resto los días y me da menor que 0, entonces no ha cumplido años.
            // Si da mayor o igual, sí ha cumplido
            if (hoy.getDate() - dia >= 0) {
                return edad + 1;
            }

            return edad;
        }
    }


};


//Callback al finalizar la carga del documento...
$(function () {

    //Remover el spinner de cargar pagina...
    $("#loading-page-spinner").fadeOut(function () {
        $(this).remove();

        //Mostrar la pagina principal...
        $("#main-card").fadeIn(function () {
            //Si se desea disparar alguna logica cuando termine la transicion de la animacion al cargar e card principal de la pagina...
        });

    });


    //#region Configuración regional para el datepicker

    // $.datepicker.regional['es'] = {
    //     closeText: 'Cerrar',
    //     prevText: '<--Ant', nextText: 'Sig-->',
    //     currentText: 'Hoy',
    //     monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    //         'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
    //     monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
    //         'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
    //     dayNames: ['Domingo', 'Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado'],
    //     dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mie;', 'Juv', 'Vie', 'Sab'],
    //     dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'],
    //     weekHeader: 'Sm',
    //     dateFormat: 'dd/mm/yy',
    //     firstDay: 1,
    //     showMonthAfterYear: false,
    //     yearSuffix: '',
    //     changeMonth: true,
    //     changeYear: true,
    //     showAnim: "slideDown",

    // };

    // ///Establecer configuración por defecto para todos los datepickers...
    // $.datepicker.setDefaults($.datepicker.regional['es']);

    // //#endregion


    //Sobreescribir la plantillla del pluggin de bootstrap notify para hacerla compatible con boostrap 5...
    $.notifyDefaults({ template: '<div data-notify="container" class="col-11 col-sm-4 alert alert-dismissible alert-{0}" role="alert"> <span data-notify="icon"></span> <span data-notify="title">{1}</span> <span data-notify="message">{2}</span><div class="progress" data-notify="progressbar"><div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div></div> <a href="{3}" target="{4}" data-notify="url"></a> <button type="button" aria-hidden="true" class="btn-close" data-notify="dismiss"></button></div>' });


    // //Establecer defaults para el pluggin jquery.blockui...
    // $.blockUI.defaults.message = '<div class="p-2 text-primary fw-bold"><span class="fas fa-spinner fa-pulse"></span> Cargando...</div>';

    // // z-index for the blocking overlay 
    // $.blockUI.defaults.baseZ = 1100;
});

var notificationInstances = [];

function showNotificationPreventDuplicates(options) {
    // Verificar si la notificación ya está presente
    if (!isNotificationDuplicated(options.message)) {
        // Agregar la instancia de notificación a la lista
        var notificationInstance = $.notify({
            message: options.message,
            icon: options.icon
        }, {
            element: options.element,
            position: options.position,
            type: options.type,
            mouse_over: 'pause',
            z_index: 1100,
            offset: {
                x: 0,
                y: 60
            },
            placement: {
                from: "top",
                align: "center"
            },
            animate: {
                enter: options.animateEnter,
                exit: options.animateExit
            },
            onClose: function () {
                // Remover la instancia de notificación de la lista cuando se cierra
                removeNotificationInstance(notificationInstance);
            }
        });

        // Agregar la instancia de notificación a la lista
        notificationInstances.push({ message: options.message, instance: notificationInstance });
    }
}

function isNotificationDuplicated(message) {
    // Verificar si la notificación ya está presente en la lista
    return notificationInstances.some(function (item) {
        return item.message === message;
    });
}

function removeNotificationInstance(instance) {
    // Remover la instancia de notificación de la lista
    var index = notificationInstances.findIndex(function (item) {
        return item.instance === instance;
    });

    if (index !== -1) {
        notificationInstances.splice(index, 1);
    }
}
