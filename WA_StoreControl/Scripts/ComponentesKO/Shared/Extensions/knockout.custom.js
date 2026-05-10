
//#region Custom ko.observables


//Agregar Observables Numericos a Knockout
ko.numericObservable = function (initialValue) {
    var _actual = ko.observable(initialValue);

    var result = ko.computed({
        read: function () {
            return _actual();
        },
        write: function (newValue) {
            var parsedValue = parseFloat(newValue);
            _actual(isNaN(parsedValue) ? newValue : parsedValue);
        }
    });

    return result;
};

//wrapper for an observable that protects value until committed
ko.protectedObservable = function (initialValue) {
    //private variables
    var _temp = initialValue;
    var _actual = ko.observable(initialValue);

    var result = ko.dependentObservable({
        read: _actual,
        write: function (newValue) {
            _temp = newValue;
        }
    }).extend({ notify: "always" }); //needed in KO 3.0+ for reset, as computeds no longer notify when value is the same

    //commit the temporary value to our observable, if it is different
    result.commit = function () {
        if (_temp !== _actual()) {
            _actual(_temp);
        }
    };

    //notify subscribers to update their value with the original
    result.reset = function () {
        _actual.valueHasMutated();
        _temp = _actual();
    };

    return result;
};


//#endregion



//#region Custom knockout Functions


//Custom function para agregar funciones de edición a un observableArray...
ko.observableArray.fn.editableItems = function (nameOfUpdateFunction) {
    nameOfUpdateFunction = nameOfUpdateFunction || "update";

    //hold the currently selected item
    this.selectedItem = ko.observable();

    //make edits to a copy
    this.itemForEditing = ko.observable();

    //populate the selected item and make a copy for editing
    this.selectItem = function (item) {
        this.selectedItem(item);
        this.itemForEditing(ko.toJS(item));
    }.bind(this);

    this.acceptItem = function (item) {
        var selected = this.selectedItem(),
            edited = ko.toJS(this.itemForEditing()); //clean copy of edited

        //apply updates from the edited item to the selected item
        selected[nameOfUpdateFunction](edited);

        //clear selected item
        this.selectedItem(null);
        this.itemForEditing(null);
    }.bind(this);

    //just throw away the edited item and clear the selected observables
    this.revertItem = function () {
        this.selectedItem(null);
        this.itemForEditing(null);
    }.bind(this);

    return this;
};


//Función para filtrar un observableArray por una propiedad dada, retorna un array con los items coincidentes...
ko.observableArray.fn.filterByProperty = function (propName, matchValue) {
    return ko.pureComputed(function () {
        var allItems = this(), matchingItems = [];
        for (var i = 0; i < allItems.length; i++) {
            var current = allItems[i];
            if (ko.unwrap(current[propName]) === matchValue)
                matchingItems.push(current);
        }
        return matchingItems;
    }, this);
}


//Verificar si se agrego un item a un array.
ko.utils.arrayIsItemAdded = function (items, item, itemsKey, itemKey) {
    try {
        if (!itemsKey) {
            throw new Error("Es necesario el valor del parametro 'itemsKey' para usar esta función.");
        }
        itemKey = itemKey || itemsKey;
        var itemFound = ko.utils.arrayFirst(items, function (i) {
            return ko.unwrap(i[itemsKey]) === ko.unwrap(item[itemKey]);
        });
        return itemFound ? true : false;

    } catch (error) {
        throw error;
    }
}


//Clona un objeto a partir de un objeto original con las propiedades especificadas.
ko.utils.cloneObject = function (object, properties) {
    try {
        //Convertir el objeto a un objeto javaScript por si nos envian un observable o con propiedades observables...
        var data = ko.toJS(object);
        var dataToSend = {};
        for (var property in data) {
            if (data.hasOwnProperty(property)) {
                if (properties.indexOf(property) >= 0 && data[property]) {
                    dataToSend[property] = data[property];
                }
            }
        }
        return dataToSend;

    } catch (error) {
        throw error;
    }
};


//#endregion



//#region Custom Bindings

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        valueUnwrapped ? $(element).hide().fadeIn() : $(element).fadeOut();// Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        // Grab some more data from another binding property
        var duration = allBindings.get('fadeDuration') || 400; // 400ms is default duration unless otherwise specified

        valueUnwrapped ? $(element).hide().fadeIn(duration) : $(element).fadeOut(duration);
    }
};


ko.bindingHandlers.slideVisible = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor()); // Get the current value of the current property we're bound to
        value ? $(element).slideDown() : $(element).slideUp(); //jQuery will hide/show the element depending on whether "value" or true or false
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        // First get the latest data that we're bound to
        var value = valueAccessor();

        // Next, whether or not the supplied model property is observable, get its current value
        var valueUnwrapped = ko.unwrap(value);

        // Grab some more data from another binding property
        var duration = allBindings.get('slideDuration') || 400; // 400ms is default duration unless otherwise specified

        // Now manipulate the DOM element
        if (valueUnwrapped)
            $(element).slideDown(duration); // Make the element visible
        else
            $(element).slideUp(duration);   // Make the element invisible
    }
};


//Custom Binding para obtener el texto de la opción seleccionada en un elemento select...
ko.bindingHandlers.selectedText = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).change(function (event) {
            var value = valueAccessor();
            var text = $(this).find("option:selected").text();
            var propertyName = allBindings.get("propertyName");
            ko.isObservable(value) ? value(text) : bindingContext.$data[propertyName] = text;
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var text = $(element).find("option:selected").text();
        var propertyName = allBindings.get("propertyName");
        ko.isObservable(value) ? value(text) : bindingContext.$data[propertyName] = text;
    }
};


//Custom Binding para obtener los elementos options de un elemento select y guardarlos en un array del viewmodel...
ko.bindingHandlers.selectElements = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var selectElement = $(element);
        var array = valueAccessor();
        selectElement.find("option").each(function (index, option) {
            if (index > 0) {
                var item = {
                    ValueMember: $(option).val(),
                    DisplayMember: $(option).text()
                }
                array.push(item);
            }
        });
    }
};


//Custom Binding para el component collapse de bootstrap...
ko.bindingHandlers.collapse = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var params = ko.unwrap(valueAccessor());
        var value = params.value;
        var target = params.target;

        $(element).click(function (e) {
            var valueUnwrapped = ko.unwrap(value);
            value(!valueUnwrapped);
        });

        $("body").on('hide.bs.collapse show.bs.collapse', target, function () {
            $(element).attr("disabled", "disabled");
        });

        $("body").on('hidden.bs.collapse shown.bs.collapse', target, function () {
            $(element).removeAttr("disabled");
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var params = ko.unwrap(valueAccessor());
        var value = params.value;
        var valueUnwrapped = ko.unwrap(value);
        var target = params.target;

        if (valueUnwrapped) {
            $(target).collapse("show");
        } else {
            $(target).collapse("hide");
        }
    }
};

//Custom Binding para el component collapse de bootstrap...
ko.bindingHandlers.bsCollapse = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        if (typeof valueUnwrapped === "object") {
            var collapser = valueUnwrapped.collapser;

            $(element).collapse(valueUnwrapped);

            $(collapser).click(function (event) {
                $(element).collapse('toggle');
            });
        }

    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        if (typeof valueUnwrapped === "object") {
        }

        if (typeof valueUnwrapped === "boolean") {
            if (valueUnwrapped) {
                $(element).collapse('show');
            } else {
                $(element).collapse('hide');
            }
        }
    }
};

//Custom Binding para inicializar el widget jquery dialog...
ko.bindingHandlers.jqDialog = {
    init: function (element, valueAccessor) {
        var options = ko.unwrap(valueAccessor()) || {};
        $(element).dialog(options);
    }
    // update: function (element, valueAccessor) { 
    //     $(element).dialog("destroy");
    //     var options = ko.unwrap(valueAccessor()) || {};
    //     $(element).dialog(options);
    // }
};


//Custom Binding para inicializar el widget jquery datepicker...
ko.bindingHandlers.jqDatepicker = {
    init: function (element, valueAccessor, allBindings) {
        var observable = valueAccessor();
        var options = ko.unwrap(allBindings.get('jqDatepicker')) || {};

        // Configuración regional en español
        $.datepicker.regional['es'] = {
            closeText: 'Cerrar',
            prevText: '< Ant',
            nextText: 'Sig >',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
                'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
                'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };
        $.datepicker.setDefaults($.datepicker.regional['es']);

        // Opciones visuales por defecto
        var defaults = {
            dateFormat: 'dd/mm/yy',
            showAnim: 'fadeIn',
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true,
            showOtherMonths: true,
            selectOtherMonths: true,
            defaultDate: new Date(),
            onSelect: function (dateText) {
                if (ko.isObservable(observable)) {
                    // Actualizamos el observable con string formateado
                    observable(dateText);
                }
            }
        };

        var settings = $.extend({}, defaults, options);
        $(element).datepicker(settings);

        // Función para parsear strings a Date
        function parseStringToDate(val) {
            if (!val) return null;

            if (val.includes('/')) {
                const [d, m, y] = val.split('/');
                return new Date(parseInt(y), parseInt(m) - 1, parseInt(d));
            }

            if (val.includes('-')) {
                return new Date(val);
            }

            return null;
        }

        // Inicializa el datepicker con el valor correcto
        var currentVal = ko.unwrap(observable);
        $(element).datepicker("setDate", parseStringToDate(currentVal));

        // Suscribirse a cambios del observable
        observable.subscribe(function (newValue) {
            $(element).datepicker("setDate", parseStringToDate(newValue));
        });
    },

    update: function (element, valueAccessor) {
        var observable = valueAccessor();
        var value = ko.unwrap(observable);

        function parseStringToDate(val) {
            if (!val) return new Date();
            if (val.includes('/')) {
                const [d, m, y] = val.split('/');
                return new Date(y, m - 1, d);
            }
            if (val.includes('-')) {
                return new Date(val);
            }
            return new Date();
        }

        $(element).datepicker("setDate", parseStringToDate(value));
    }
};


//Custom Binding para inicializar el widget jquery tab...
ko.bindingHandlers.jqTabs = {
    init: function (element, valueAccessor) {
        var options = ko.unwrap(valueAccessor()) || {};
        $(element).tabs(options);
    }
};

//Indica si se ha hecho clic en un elemento o dentro de los hijos del mismo...
ko.bindingHandlers.hasClickWithin = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        $(document).click(function (event) {
            var target = $(event.target);
            if (target.closest(element).length > 0) {
                value(true);
            } else {
                value(false);
            }
        });

    }
};


//Custom Binding para el componente progresBar de bootstrap...
ko.bindingHandlers.bsProgressBar = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);

        var width = valueUnwrapped <= 2 ? "3%" :
            valueUnwrapped > 100 ? "100%" :
                valueUnwrapped + "%";

        var elem = $(element);
        var bgColor = allBindings.get("bsColor") || "bg-primary";
        elem.addClass("progress");
        var progressElement = $("<div></div>");
        progressElement.addClass("progress-bar");
        progressElement.addClass(bgColor);
        progressElement.attr("role", "progressbar");
        progressElement.css("width", width);

        progressElement.text(valueUnwrapped + "%");

        elem.append(progressElement);
    },

    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        var width = valueUnwrapped <= 2 ? "3%" :
            valueUnwrapped > 100 ? "100%" :
                valueUnwrapped + "%";

        var elem = $(element);
        var progressElement = elem.find("div:first");
        progressElement.css("width", width);
        progressElement.text(valueUnwrapped + "%");

    }
};


//Custom Binding para el componente Toggle de bootstrap...
ko.bindingHandlers.bsToggle = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        var options = allBindings.get("toggleOptions" || { onlabel: "on", offlabel: "off" });
        $(element).bootstrapToggle(options);

        //Register an event handler to listen changes on input element
        $(element).change(function (event) {
            var checked = $(this).is(':checked')
            value(checked);
        });

    },

    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        var onOff = valueUnwrapped ? "on" : "off";
        $(element).bootstrapToggle(onOff);
    }
};

//Custom Binding para inicializar el widget jquery checkboxradio...
ko.bindingHandlers.jqCheckboxradio = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor()) || {};
        $(element).checkboxradio(options); //Inicializar widget...

        var value = allBindings.get("checked"); //Obtener el checked binding...
        var valueUnwrapped = ko.unwrap(value); //unwrap value...

        //Checkar el input en inicialmente...
        var isChecked = $(element).val() === valueUnwrapped;
        $(element).prop("checked", isChecked);
        $(element).checkboxradio("refresh");

        //Subscribirse al checked binding para actualizar el estado del widget...
        if (ko.isObservable(value)) {
            value.subscribe(function (newValue) {
                $(element).prop("checked", $(element).val() === newValue);
                $(element).checkboxradio("refresh");
            });
        }
    }
};


//Custom Binding para el component popover de bootstrap...
ko.bindingHandlers.bsPopover = {
    init: function (element, valueAccessor) {
        var options = ko.unwrap(valueAccessor());
        new bootstrap.Popover(element, options)
    }
};


//Custom Binding para el component popover de bootstrap...
ko.bindingHandlers.bsTooltip = {
    init: function (element, valueAccessor) {
        var options = ko.unwrap(valueAccessor());
        if (options.disable) {
            var wrapper = document.createElement("span");
            wrapper.setAttribute("tabindex", "0");
            element.parentNode.insertBefore(wrapper, element);
            wrapper.appendChild(element);
            element.setAttribute("style", "pointer-events: none;");
            $(wrapper).tooltip(options);

        } else {
            $(element).tooltip(options);
        }
    }
};

//Custom Binding para asignar un id unico a un elemento...
ko.bindingHandlers.uniqueId = {
    init: function (element, valueAccessor) {
        if (valueAccessor()) {
            var id = "ko_unique_id_" + (++ko.bindingHandlers['uniqueId'].currentIndex);
            element.id = id;
        }
    }

}; ko.bindingHandlers['uniqueId'].currentIndex = 0;


//Custom Binding para parsear un formulario y aplicar las reglas de jquery.validete.unobstrusive...
ko.bindingHandlers.jqValidateParseForm = {
    init: function (element, valueAccessor) {
        AppGlobal.ParseDynamicContent(element);
    }
};


//Custom Binding para jquery inputmask pluggin...
ko.bindingHandlers.jqInputmask = {
    init: function (element, valueAccessor, allBindings) {
        var mask = valueAccessor();
        $(element).inputmask(mask);

        if (allBindings.has('text')) { //Si el elemento tiene el binding de text y esta enlazado a un observable... 
            var textBinding = allBindings.get('text');
            if (ko.isObservable(textBinding)) {
                textBinding.subscribe(function (value) { //Nos subscribimos manualmente al observable...
                    $(element).inputmask('remove'); //para remover la mask anterior...
                    $(element).inputmask(mask); //...y volver a aplicar la mask cada vez que el valor subyacente se actualice.
                });
            }
        }
    }
};

/*//Custom Binding para inicializar el jquery plugin smartWizard...
ko.bindingHandlers.jqSmartWizard = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor()) || {};
        $(element).smartWizard(options); //Inicializar widget... 

        //Inicializar eventos...
        if (options.leaveStep && typeof options.leaveStep === "function") {
            $(element).on("leaveStep", options.leaveStep);  //Inicializar widget... 
        }
    }
};*/

//Custom Binding para jquery smartwizard pluggin...
ko.bindingHandlers.jqSmartwizard = {
    init: function (element, valueAccessor, allBindings) {
        var options = valueAccessor();
        var stepNumberParam = allBindings.has('saveStepNumberOn') ? allBindings.get('saveStepNumberOn') : 0;
        var triggerFormValidations = allBindings.has('validateForm') ? allBindings.get('validateForm') === true : false;


        $(element).smartWizard(options);

        $(element).on("showStep", function (e, anchorObject, stepNumber, stepDirection) {
            if (ko.isObservable(stepNumberParam))
                stepNumberParam(stepNumber);
            else
                stepNumberParam = stepNumber;

            return true;
        })

        //dispárar las valdiaciones cuando se mande en true la bandera
        $(element).on("leaveStep", function (e, anchorObject, stepNumber, stepDirection) {
            if (triggerFormValidations) {
                var form = e.currentTarget.closest("form");

                return $(form).valid();
            }
            return true;
        })
    }
};


//Custom Binding para inicializar el componente modal de bootstrap...
ko.bindingHandlers.bsModal = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor()) || {};
        $(element).modal(options); //Inicializar widget... 

        //Inicializar eventos...
        if (options.hiddenModal && typeof options.hiddenModal === "function") {
            $(element).on("hidden.bs.modal", options.hiddenModal);  //Inicializar widget... 
        }
    }
};

ko.bindingHandlers.jqMask = {
    init: function (element, valueAccessor, allBindings) {
        var options = ko.unwrap(valueAccessor()) || {};
        var mask = options.Mask;
        var config = ko.unwrap(options.Config) || {};
        var maskUnwrapped = ko.unwrap(options.Mask);
        var currentMask = maskUnwrapped;
        var unmaskOn = options.UnmaskOn;

        // Forzar type=text para permitir comas y máscara
        if (element.type && element.type.toLowerCase() === "number") {
            element.type = "text";
        }

        // Inicializar máscara
        $(element).mask(maskUnwrapped, config);

        // Formatear valor inicial del observable si existe
        var valueBinding = allBindings.get("value");
        if (ko.isObservable(valueBinding)) {
            var initialVal = ko.unwrap(valueBinding);
            if (initialVal !== null && initialVal !== undefined && initialVal !== "") {
                var strVal = typeof initialVal === "number" ? initialVal.toFixed(2) : initialVal;
                $(element).val(strVal).trigger('input');
            }
        }

        // Suscribirse a cambios de máscara si es observable
        if (ko.isObservable(mask)) {
            mask.subscribe(function (newMask) {
                currentMask = newMask;
                if (newMask) {
                    $(element).mask(newMask, config);
                    $(element).val($(element).val()).trigger('input');
                } else {
                    $(element).unmask();
                }
            });
        }

        // Actualizar observable sin máscara
        if (unmaskOn && ko.isObservable(unmaskOn)) {
            $(element).on("focusout change", function () {
                var raw = $(element).val() || "";

                // Remover comas y convertir a número
                raw = raw.replace(/,/g, '');
                var num = parseFloat(raw);
                unmaskOn(isNaN(num) ? null : num);
            });
        }

        // Suscribirse a cambios del observable value para actualizar la máscara
        if (ko.isObservable(valueBinding)) {
            valueBinding.subscribe(function (newVal) {
                if (newVal === null || newVal === undefined) {
                    $(element).val('');
                } else {
                    var strVal = typeof newVal === "number" ? newVal.toFixed(2) : newVal;
                    $(element).val(strVal).trigger('input');
                }
            });
        }
    }
};

//Custom Binding para inicializar el jquery autoResize...
ko.bindingHandlers.jqAutoResize = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = ko.unwrap(valueAccessor()) || {};
        //Inicializar plugin y disparar el autoresize...
        $(element).autoResize(options);
        $(element).trigger('change.dynSiz');
    }
};


ko.bindingHandlers.AutoResizeTextarea = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).on('input', function () {
            AppGlobal.AutoResizeTextarea(element);
        });
    }
};

//Custom Binding para inicializar el bootstrap select...
ko.bindingHandlers.bsSelect = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var settings = ko.unwrap(valueAccessor()) || {};
        $(element).selectpicker(settings);

        var options = allBindings.get("options");
        if (options && ko.isObservable(options)) {
            options.subscribe(function (newValue) {
                $(element).selectpicker("refresh");
            });
        }
    }
};

//Custom Binding para cambiar el comportamiento del binding html que no aplica bindings a los elementos inyectados al DOM...
ko.bindingHandlers.htmlBound = {
    init: function () {
        return { controlsDescendantBindings: true };
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        ko.utils.setHtml(element, valueAccessor());
        ko.applyBindingsToDescendants(bindingContext, element);
    }
};

//Custom Binding agregar un observable para conocer la longitud actual y disponible de una propiedad de texto observable...
ko.bindingHandlers.CurrentLength = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var maxlen = parseInt($(element).data("val-length-max")); //Obtener la longitud maxima del elemento generada por el modelo mvc...
        var options = ko.unwrap(valueAccessor());
        var value = allBindings.get("value") || allBindings.get("textInput"); //Obtener el valor actual de la cadena en base al binding value o textInput...
        var valueUnwrapped = ko.unwrap(value);
        options.Prop.MaxLength = ko.observable(maxlen);
        options.Prop.CurrentLength = ko.observable(valueUnwrapped.length);

        if (!isNaN(maxlen) && allBindings.has('textInput') || allBindings.has('value')) {

            $(element).on("input", function (event) {
                options.Prop.CurrentLength(event.target.value.length);
            });

            options.Prop.AvailableLength = ko.computed(function () {
                let availableLength = (options.Prop.MaxLength() - options.Prop.CurrentLength());
                return availableLength < 0 ? 0 : availableLength;
            });

            if (ko.isObservable(value)) {
                value.subscribe(function (newVal) {
                    options.Prop.CurrentLength(newVal.length);
                });

            }
        };
    }
};

//Custom Binding para inicializar el valor de una propiedad en base al valor inicial del elemento html al que se enlace...
ko.bindingHandlers.valueWithInit = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
        var property = valueAccessor(),
            value = element.value;

        //create the observable, if it doesn't exist 
        if (!ko.isWriteableObservable(viewModel[property])) {
            viewModel[property] = ko.observable();
        }

        viewModel[property](value);

        ko.applyBindingsToNode(element, { value: viewModel[property] }, context);
    }
};


//Custom Binding para inicializar el valor de RequestVerificationToken a una propiedad del viewModel...
ko.bindingHandlers.RequestVerificationToken = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
        var property = valueAccessor();
        var token = $(element).find("input").first().val();
        if (ko.isWriteableObservable(property)) {
            property(token);
        } else {
            property = token;
        }
    }
};


//Custom Binding para validar selecciones unicas de responsables...
ko.bindingHandlers.uniqueResponsable = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
        var options = valueAccessor();
        $(element).addClass(options.RuleName);
        $.validator.addMethod(options.RuleName, options.OnValidate, options.ErrorMessage);
    }
};


//Custom Binding para seleccionar un elemento un select si el arreglo subyacente solo contiene un unico item...
ko.bindingHandlers.PreSelectIfUniqueInArray = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        const selectIfUnique = (arr, propertyName) => { //Definir una funcion para reutilizarla en caso de que la propiedad enlazada al options binding sea un observable...
            let unwrappedArr = ko.unwrap(arr);
            let value = allBindings.get('value'); //Obtener la propiedad al cual esta enlazada el value binding...

            if (unwrappedArr && Array.isArray(unwrappedArr) && unwrappedArr.length === 1) { //Determinar si se esta usando el options binding, si la propiedad enlazada es un arreglo y si solo contiene un elemento...
                //Si hay un nombre de propiedad entonces acceder al valor, si no tomar el valor completo del objeto en el array...
                let selectedValue = propertyName ? ko.unwrap(unwrappedArr[0][propertyName]) : unwrappedArr[0];
                value(selectedValue); //Actualizar el valor de la propiedad enlazada al value binding...
            }
        };

        let arr = allBindings.get('options'); //Obtener el array al cual esta enlaza el options binding...
        let propertyName = allBindings.get("optionsValue"); //Obtener el nombre de la propiedad al cual esta enlazada el optionsValue, si la hay...

        selectIfUnique(arr, propertyName);
        if (ko.isObservableArray(arr)) {
            arr.subscribe(newArr => {
                selectIfUnique(newArr, propertyName);
            });
        }
    }
};


//Custom Binding para crear una subpropiedad HasFocus...
ko.bindingHandlers.addHasFocusProp = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let value = ko.unwrap(valueAccessor());
        let property = allBindings.get("value") || allBindings.get("textInput");
        property.HasFocus = ko.observable(value);
    }
};

//Custom Binding para inicializar el plugin jquery de select2...
ko.bindingHandlers.jqSelect2 = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let options = ko.unwrap(valueAccessor()); //Obtener las opciones de configuracion...
        let valueProp = allBindings.get("value"); //Obtener la propiedad a la que esta enlazada el value binding en el select...

        $(element).select2(options); //Inicializar el plugin...

        if (ko.isObservable(valueProp)) { //Si el value se esta usando sobre una propiedad observable...
            valueProp.subscribe((v) => { //Subscribirnos al cambio para actualizar el select cuando se cambia programaticamente el valor...
                if ($(element).val() != v) {
                    $(element).val(v);
                    $(element).trigger('change');
                }
            });
        }
    },//ustom binding también responda a actualizaciones (update) y no solo init, para que cuando Knockout cambie el valor o las opciones, Select2 refresque correctamente.
    update: function (element, valueAccessor, allBindings) {
        let valueProp = allBindings.get("value");
        let value = ko.unwrap(valueProp);

        // Refrescar visualmente la selección
        $(element).val(value).trigger('change.select2');
    }
};

//Custom Binding para deshabilitar elementos hijos ...
ko.bindingHandlers.DisableAllChildren = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let disable = ko.unwrap(valueAccessor());
        $(element).find("input, select, textarea").each(function (index, e) {
            $(e).prop('disabled', disable);
        });
    }
};

//Custom Binding para option label con valor y texto personalizado...
ko.bindingHandlers.optionsLabel = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let optionLabelText = ko.unwrap(valueAccessor()); //Obtener el text del label...
        let labelOption = $(document.createElement("option")); //Crear el nodo option para el label...
        let firstOption = $(element).find("option:first-child"); //Recuperar la primera option del select...

        //Settear valores por defecto del label...
        labelOption.val("")
        labelOption.text(optionLabelText);

        //Insertar el option de primero...
        $(labelOption).insertBefore(firstOption);
    }
};

//Custom Binding para cancelar el bubbling de cualquier elemento ...
ko.bindingHandlers.stopBubble = {
    init: function (element, valueAccessor) {
        ko.utils.registerEventHandler(element, "click", function (event) {
            event.cancelBubble = true;
            if (event.stopPropagation) {
                event.stopPropagation();
            }
        });
    }
};

//Custom Binding Auto Ajustar altura de textarea...
ko.bindingHandlers.ResizeTextarea = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).on('input focus', function () {
            AppGlobal.ResizeTextarea(element);
        });
    }
};


//Registrar custom binding para deshabilitar todos los elementos de entra de usuario...
ko.bindingHandlers.disableInputs = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let disabled = ko.unwrap(valueAccessor()); //Bandera para habilitar o deshabilitar....
        $(element).find("input, select, textarea, button, a").each(function () {
            if (!$(this).hasClass("exclude-disabled")) {
                if (disabled) {
                    $(this).attr("disabled", "disabled");
                    $(this).addClass("disabled");

                } else {
                    $(this).removeAttr("disabled");
                    $(this).removeClass("disabled");
                }
            }
        });
    }
};

//Custom extender para strings en solo mayuscula...
ko.extenders.onlyUpperCase = function (target) {
    //create a writable computed observable to intercept writes to our observable
    let result = ko.pureComputed({
        read: target,  //always return the original observables value
        write: function (newValue) {
            if (typeof (newValue) == "string") {
                target(newValue.toUpperCase());
            } else {
                target(newValue);
            }
        }
    }).extend({ notify: 'always' });

    //initialize with current value to make sure it is rounded appropriately
    result(target());

    //return the new computed observable
    return result;
};

//Custom Binding Para Crear tus propias reglas de validacion del lado del cliente...
ko.bindingHandlers.ValidateRules = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
        var options = valueAccessor();
        options.forEach(op => {
            $(element).addClass(op.RuleName);
            $.validator.addMethod(op.RuleName, op.OnValidate, op.ErrorMessage);
        });
    }
};

//Custom Binding para obtener la lista disponible de elementos por tipo...
ko.bindingHandlers.GetSelectOptionsFromElementoPorTipo = {
    init: function (element, valueAccessor, allBindings, viewModel, context) {
        var options = valueAccessor();
        var defaultValue = ko.unwrap(options.defaultValue);

        var beforeSend = (jqXHR) => {  // Petición en curso
            options.cargandoOrigenListaIndicator(true);
            //ya que, el mismo input que se desea ocultar/mostrar es quien le indica el comportamiento al indicador de cargando 
        };

        var success = (response) => { // Respuesta de la petición
            if (response.Success) {
                options.origenDeListaDatos(response.Record);
                options.value(defaultValue);
            }
            else {
                AppGlobal.ShowNotification({
                    element: "body",
                    message: response.Message,
                    type: "danger",
                    icon: "fas fa-exclamation-triangle",
                    animateEnter: "animated bounceIn",
                    animateExit: "animated bounceOut"
                });
            }
        };

        var error = () => { // Error de la peticion
            AppGlobal.ShowNotification({
                element: "body",
                message: AppGlobal.Messages.AjaxRequestError,
                type: "danger",
                icon: "fas fa-exclamation-triangle",
                animateEnter: "animated bounceIn",
                animateExit: "animated bounceOut"
            });
        };

        AppGlobal.Ajax.GetDatosForOrigenLista({
            data: { origenDeLista: options.origenDeLista() },
            beforeSend: beforeSend,
            complete: () => { options.cargandoOrigenListaIndicator(false); },
        }).done(success).fail(error);
    }
};

//Custom Binding Para crear la instancia del buscador correspondiente para aquellos elementos por tipo que lo necesitaran...
ko.bindingHandlers.GetInstanceForElementoPorTipo = {
    init: function (element, valueAccessor, allBindings, viewModel, context) {
        var options = valueAccessor();
        var observableName = options.elementoPorTipoName + 'Applied' + ko.unwrap(options.elementoPorTipoid);

        var elemento = options.elementosPorTipoFinders().find(x => x[options.elementoPorTipoid]);

        //si en el root aun no se encuentra el observable entonces declararlo, sino trabajar con el mismo obsevable
        if (!ko.isObservable(context.$root[observableName]))
            context.$root[observableName] = ko.observable();

        if (elemento) {
            let parameters = { ...elemento[options.elementoPorTipoid]['parameters'], ...options.instanceParameters };
            let selectorName = elemento[options.elementoPorTipoid]['selector'];//selector html del componente
            let instance = new elemento[options.elementoPorTipoid]['name'](parameters || {});//instancia del componente pasandole los parametros con los que va a trabajar
            context.$root[observableName](instance);

            let elem = `<${selectorName} params="viewModel:${observableName}"></${selectorName}>`;//widget del componente parametrizado que se debe insertar al DOM en el Index.cshtml

            options.ElementsApplied.push(elem);

            ko.utils.registerEventHandler(element, "click", function (event) {
                // instance.ModalBootstrapInstance().show();
                context.$root[observableName]().ModalBootstrapInstance().show();
            });
        }
    }
};

//Custom binding para cargar elementos por tipo que dependen de otro elemento padre
ko.bindingHandlers.SetSubscriptionForElementoPorTipo = {
    init: function (element, valueAccessor, allBindings, viewModel, context) {
        var options = valueAccessor();
        var elementoPorTipo = options.elementoPorTipo;
        var LoadingAjaxCurrentRequest = ko.observable(null);

        if (ko.unwrap(elementoPorTipo.ElementoVinculadoId)) {
            var elementoVinculado = options.allElementos().find(x => ko.unwrap(x.ElementoPorTipoId) == ko.unwrap(elementoPorTipo.ElementoVinculadoId));

            elementoPorTipo.Valor.subscribe((v) => {
                var beforeSend = (jqXHR) => {  // Petición en curso
                    // options.cargandoOrigenListaIndicator(true);
                    if (LoadingAjaxCurrentRequest()) //verifica si hay otra petición para abortar
                        LoadingAjaxCurrentRequest().abort();

                    LoadingAjaxCurrentRequest(jqXHR);
                };
                var success = (response) => { // Respuesta de la petición
                    if (response.Success) {
                        elementoVinculado.OrigenListaDatos(response.Record);//mostrar los ultimos agregados primero
                    }
                    else {
                        AppGlobal.ShowNotification({
                            element: "body",
                            message: response.Message,
                            type: "danger",
                            icon: "fas fa-exclamation-triangle",
                            animateEnter: "animated bounceIn",
                            animateExit: "animated bounceOut"
                        });
                    }
                };

                var error = (jqXHR, statusText) => { // Error de la peticion
                    if (statusText !== "abort") {
                        AppGlobal.ShowNotification({
                            element: "body",
                            message: AppGlobal.Messages.AjaxRequestError,
                            type: "danger",
                            icon: "fas fa-exclamation-triangle",
                            animateEnter: "animated bounceIn",
                            animateExit: "animated bounceOut"
                        });
                    }
                };

                AppGlobal.Ajax.GetDatosForOrigenLista({
                    data: { origenDeLista: elementoVinculado.OrigenLista(), value: ko.unwrap(v) },
                    beforeSend: beforeSend,
                    complete: () => { LoadingAjaxCurrentRequest(null); },
                }).done(success).fail(error);
            });
        }
    }
};

//Custom binding para aplicar una clase al elemento o a otro elemento deseado
ko.bindingHandlers.applyClass = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = valueAccessor();

        if (ko.unwrap(options.classToApply)) {
            var target = options.target || element;
            target.addClass(options.classToApply() || options.classToApply);//add class initialy and then just when the class change

            $(element).classChange((el, newClass) => {
                target.addClass(options.classToApply() || options.classToApply);
            });
        }
    }
};

//Custom Binding para renderizar un archivo multimedia local o desde el servidor
ko.bindingHandlers.renderMultimedia = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        if (allBindings.has("type") && allBindings.has("src")) {
            if (allBindings.get("type") === 'local') {
                let src = allBindings.get("src");
                $(element).attr("src", 'data:' + ko.unwrap(src.ContentType) + ';base64,' + ko.unwrap(src.Content));
            }
            if (allBindings.get("type") === 'fromServer') {
                if (allBindings.has("callback")) {
                    let callback = allBindings.get("callback");
                    callback.call();
                }
                else {
                    console.log("Debe especificar el callback");
                }
            }
        }
        else
            console.log("Las propiedades type y src son obligatorias");
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        if (allBindings.has("type") && allBindings.has("src")) {
            if (allBindings.get("type") === 'local') {
                let src = allBindings.get("src");
                $(element).attr("src", 'data:' + ko.unwrap(src.ContentType) + ';base64,' + ko.unwrap(src.Content));
            }
            if (allBindings.get("type") === 'fromServer') {
                let src = allBindings.get("src");

                if (ko.unwrap(src.ContentType).length > 0 && ko.unwrap(src.Content).length > 0) {
                    $(element).attr("src", 'data:' + ko.unwrap(src.ContentType) + ';base64,' + ko.unwrap(src.Content));
                }
            }
        }
        else
            console.log("Las propiedades type y src son obligatorias");
    }
};

//CUSTOM BINDING PARA DESENCADENAR EL EVENTO KEYPRESS DEL ELEMENTO HTML
ko.bindingHandlers.keyPressAction = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        const params = valueAccessor();

        element.addEventListener("keydown", function (e) {
            const key = params.key || "Enter";
            if (e.key === key) {
                if (params.preventDefault !== false) e.preventDefault();

                // Forzar que Knockout actualice el observable value antes de ejecutar la acción
                const valueBinding = allBindings.get("value");
                if (ko.isObservable(valueBinding)) {
                    valueBinding(element.value);
                }

                const accion = params.action;
                if (typeof accion === "function") {
                    accion.call(viewModel, e); // pasa el evento si se necesita
                }
            }
        });
    }
};

//Custom Binding para inicializar el plugin jquery de select2 con busqueda de datos interactiva
ko.bindingHandlers.select2Ajax = {
    init: function (element, valueAccessor) {
        let options = valueAccessor();
        let $element = $(element);
        let dropdownParent = null;

        if (ko.unwrap(options.dropdownParent))
            dropdownParent = $(ko.unwrap(options.dropdownParent));
        else {
            let $modal = $element.closest('.modal');

            if ($modal.length)
                dropdownParent = $modal;
        }

        let idField = ko.unwrap(options.idField) || 'Id';
        let textFields = ko.unwrap(options.textFields) || ['Nombre'];
        let textSeparator = ko.unwrap(options.textSeparator) || ' || ';
        let queryParam = ko.unwrap(options.queryParam) || 'q';

        let mapResult = options.mapResult || function (item) {
            return {
                id: item[idField],
                text: textFields.map(function (f) {
                    return item[f] || '';
                }).join(textSeparator)
            };
        };

        var recordKey = ko.unwrap(options.recordKey) || 'Record';

        var getRecords = options.getRecords || function (data) {
            return data[recordKey] || [];
        };

        var select2Options = {
            width: ko.unwrap(options.width) || '100%',
            placeholder: ko.unwrap(options.placeholder) || 'Seleccione...',
            allowClear: ko.unwrap(options.allowClear) !== false,
            minimumInputLength: ko.unwrap(options.minimumInputLength) ?? 3,
            language: ko.unwrap(options.language) || {
                inputTooShort: function (args) {
                    return 'Escriba al menos ' + args.minimum + ' caracteres';
                },
                searching: function () {
                    return 'Buscando…';
                },
                noResults: function () {
                    return 'No se encontraron resultados';
                }
            },

            ajax: {
                url: ko.unwrap(options.url),
                dataType: ko.unwrap(options.dataType) || 'json',
                delay: ko.unwrap(options.delay) ?? 400,
                data: function (params) {
                    var extra = ko.unwrap(options.extraParams) || {};
                    var query = {};
                    query[queryParam] = params.term;

                    return $.extend({}, query, extra);
                },
                beforeSend: function () {
                    $element.closest('.select2-container')
                        .find('.select2-search__field')
                        .prop('disabled', true);
                },
                complete: function () {
                    $element.closest('.select2-container')
                        .find('.select2-search__field')
                        .prop('disabled', false);
                },
                processResults: function (data) {
                    return {
                        results: getRecords(data).map(mapResult),
                        pagination: {
                            more: !!data.more
                        }
                    };
                },
                cache: ko.unwrap(options.cache) !== false
            }
        };

        if (dropdownParent && dropdownParent.length)
            select2Options.dropdownParent = dropdownParent;

        $element.select2(select2Options);

        if (options.value && ko.isObservable(options.value)) {
            $element.on('select2:select select2:unselect', function () {
                options.value($element.val());
            });

            options.value.subscribe(function (newVal) {
                $element.val(newVal)
                    .trigger('change.select2');
            });
        }

        var applyItem = function (item) {
            if (!item)
                return;

            var mapped = (item.id != null && item.text != null) ? item : mapResult(item);

            var exists = $element.find('option[value="' + mapped.id + '"]').length;

            if (!exists) {
                var option = new Option(mapped.text, mapped.id, true, true);

                $element.append(option);
            }

            $element.val(mapped.id).trigger('change.select2');

            if (options.value && ko.isObservable(options.value))
                options.value(mapped.id);
        };

        if (options.selectedItem) {
            var initial = ko.unwrap(options.selectedItem);

            if (initial)
                applyItem(initial);

            if (ko.isObservable(options.selectedItem))
                options.selectedItem.subscribe(applyItem);
        }

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            if ($element.data('select2'))
                $element.select2('destroy');

            $element.off('select2:select select2:unselect');
        });
    },

    update: function (element, valueAccessor) {
        var options = valueAccessor();
        var $element = $(element);

        if (!options.selectedItem)
            return;

        var item = ko.unwrap(options.selectedItem);

        if (!item)
            return;

        var exists = $element.find('option[value="' + item.id + '"]').length;

        if (!exists) {
            var option = new Option(item.text, item.id, true, true);

            $element.append(option);
        }

        $element.val(item.id).trigger('change.select2');
    }
};

let formato = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
});
//#endregion
