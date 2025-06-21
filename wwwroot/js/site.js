// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var detallesData
$(document).ready(function () {
    
    $(".chosen-select").chosen({
        no_results_text: "No hay resultados...",
        search_contains:true
    });
    var detalles = detallesData ?? null; 

    var detalleIndex = 0;

    $("#agregarDetalleBtn").on("click", function () {

        var articuloId = $('#articuloSelect').val();
        var articuloText = $('#articuloSelect option:selected').text();
        var loteId = $('#loteSelect').val();
        var loteText = $('#loteSelect option:selected').text();
        var cantidad = $('#cantidad').val();
        var marca = $('#marcaSelect option:selected').text();


        if ((cantidad != '' && cantidad != '0') && (articuloId != '' && articuloId != '0')) {
            $('#articulo-error').text("");
            $('#cantidad-error').text("");

            // Verificar si la combinación de articuloId y loteId ya existe en la tabla
            var exists = false;
            $('#detallesTable tbody tr').each(function () {
                var existingArticuloId = $(this).find('input[name*=".ArticuloId"]').val();
                var existingLoteId = $(this).find('input[name*=".LoteId"]').val();

                if (existingArticuloId == articuloId && existingLoteId == loteId) {
                    exists = true;
                    return false; // Salir del bucle each
                }
            });
            if (!exists) {
                var detalleHtml = `
                        <tr>
                            <td>
                                <input type="hidden" name="detalleIngresos[${detalleIndex}].ArticuloId" value="${articuloId}" />
                                 <input type="hidden" name="detalleIngresos[${detalleIndex}].Articulo" value="${articuloText}" />
                                ${articuloText}
                            </td>
                            <td>
                                <input type="hidden" name="detalleIngresos[${detalleIndex}].Marca" value="${marca}" />
                                ${marca}
                            </td>
                            <td>
                                <input type="hidden" name="detalleIngresos[${detalleIndex}].LoteId" value="${loteId}" />
                                 <input type="hidden" name="detalleIngresos[${detalleIndex}].Lote" value="${loteText}" />
                                ${loteText}
                            </td>                       
                            <td>
                                <input type="hidden" name="detalleIngresos[${detalleIndex}].Cantidad" value="${cantidad}" />
                                ${cantidad}
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger btn-sm" onclick="$(this).closest('tr').remove();">Eliminar</button>
                            </td>
                        </tr>
                    `;

                $('#detallesTable tbody').append(detalleHtml);
                detalleIndex++;
            } else {
                alert('Este artículo y lote ya están en la lista.');
            }
        }
        else {
            if ((cantidad == '' || cantidad == '0') && (articuloId == '' || articuloId == '0')) {
                $('#articulo-error').text("Debe seleccionar un Articulo");
                $('#cantidad-error').text("La cantidad es obligatoria");
            }
            else if (articuloId == '' || articuloId == '0') {
                $('#articulo-error').text("Debe seleccionar un Articulo");
                $('#cantidad-error').text("");
            }
            else if (cantidad == '' || cantidad == '0') {
                $('#cantidad-error').text("La cantidad es obligatoria");
                $('#articulo-error').text("");
            }
        }
    });


    if (detalles != null) {

    detalles.forEach(function (detalle, index) {
        var detalleHtml = `
                <tr>
                    <td>
                        <input type="hidden" name="detalleIngresos[${index}].ArticuloId" value="${detalle.articuloId}" />
                        <input type="hidden" name="detalleIngresos[${index}].Articulo" value="${detalle.articulo}" />
                        ${detalle.articulo}
                    </td>
                    <td>
                        <input type="hidden" name="detalleIngresos[${index}].Marca" value="${detalle.marca}" />
                        ${detalle.marca}
                    </td>
                    <td>
                       <input type="hidden" name="detalleIngresos[${index}].LoteId" value="${detalle.loteId === 'null' || detalle.loteId === null ? '' : detalle.loteId}" />
                        <input type="hidden" name="detalleIngresos[${index}].Lote" value="${detalle.lote}" />
                        ${detalle.lote}
                    </td>
                    <td>
                        <input type="hidden" name="detalleIngresos[${index}].Cantidad" value="${detalle.cantidad}" />
                        ${detalle.cantidad}
                    </td>
                    <td>
                        <button type="button" class="btn btn-danger btn-sm" onclick="$(this).closest('tr').remove();">Eliminar</button>
                    </td>
                </tr>
            `;

        $('#detallesTable tbody').append(detalleHtml);
        detalleIndex++;
    });

    }
    var detalleEgresoIndex = 0;
    
    $('#agregarEgresoBtn').on("click", function () {

        var articuloId = $('#articuloSelect').val();
        var articuloText = $('#articuloSelect option:selected').text();
        var loteId = $('#loteSelect').val();
        var loteText = $('#loteSelect option:selected').text();
        var cantidad = $('#cantidad').val();
        var marca = $('#marcaSelect option:selected').text();        


        if ((cantidad != '' && cantidad != '0') && (articuloId != '' && articuloId != '0')) {
            $('#articulo-error').text("");
            $('#cantidad-error').text("");

            // Verificar si la combinación de articuloId y loteId ya existe en la tabla
            var exists = false;
            $('#detallesTable tbody tr').each(function () {
                var existingArticuloId = $(this).find('input[name*=".ArticuloId"]').val();
                var existingLoteId = $(this).find('input[name*=".LoteId"]').val();

                if (existingArticuloId == articuloId && existingLoteId == loteId) {
                    exists = true;
                    return false; // Salir del bucle each
                }
            });
            if (!exists) {
                var detalleHtml = `
                        <tr>
                            <td>
                                <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].ArticuloId" value="${articuloId}" />
                                 <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].Articulo" value="${articuloText}" />
                                ${articuloText}
                            </td>
                            <td>
                                <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].Marca" value="${marca}" />
                                ${marca}
                            </td>
                            <td>
                                <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].LoteId" value="${loteId}" />
                                 <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].Lote" value="${loteText}" />
                                ${loteText}
                            </td>                       
                            <td>
                                <input type="hidden" name="detalleEgresos[${detalleEgresoIndex}].Cantidad" value="${cantidad}" />
                                ${cantidad}
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger btn-sm" onclick="$(this).closest('tr').remove();">Eliminar</button>
                            </td>
                        </tr>
                    `;

                $('#detallesTable tbody').append(detalleHtml);
                detalleEgresoIndex++;
            } else {
                alert('Este artículo y lote ya están en la lista.');
            }
        }
        else {
            if ((cantidad == '' || cantidad == '0') && (articuloId == '' || articuloId == '0')){
                $('#articulo-error').text("Debe seleccionar un Articulo");
                $('#cantidad-error').text("La cantidad es obligatoria");
            }
            else if (articuloId == '' || articuloId == '0') {
                $('#articulo-error').text("Debe seleccionar un Articulo");
                $('#cantidad-error').text("");
            }
            else if (cantidad == '' || cantidad == '0') {
                $('#cantidad-error').text("La cantidad es obligatoria");
                $('#articulo-error').text("");
            }
        }

        
    });

    $('#rubroSelect').on("change", function () {
        $("#stock").val('') ;
        var $marcaSelect = $('#marcaSelect');
        var $articuloSelect = $('#articuloSelect');
        $marcaSelect.empty();
        var idRubro = $(this).val();
        if (idRubro) {
            var url = '/Articulo/GetMarcasPorRubro';
            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                data: { idRubro: idRubro },
                success: function (data) {

                    $('#articuloSelect').prop('disabled', false);
                    $articuloSelect.append('<option value="">Seleccione un artículo</option>');
                    $.each(data.articulos, function (index, articulo) {
                        $articuloSelect.append('<option value="' + articulo.idArticulo + '">' + articulo.nombre + '</option>');
                    });
                    $('#articuloSelect').trigger("chosen:updated");


                    $('#marcaSelect').prop('disabled', false);
                    $marcaSelect.append('<option value="">Seleccione una Marca</option>');
                    $.each(data.marcas, function (index, marca) {
                        $marcaSelect.append('<option value="' + marca.idMarca + '">' + marca.marcaName + '</option>');
                    });
                    $('#marcaSelect').trigger("chosen:updated");
                },
                error: function (xhr, status, error) {
                    console.log('Error: ' + error);
                }
            });
        } else {
            $('#marcaSelect').empty().append('<option value="">Seleccione una Marca</option>');
        }
    });

    $('#marcaSelect').on("change", function () {
        $("#stock").val('') 
        var $articuloSelect = $('#articuloSelect');
        $articuloSelect.empty();
        var idMarca = $(this).val();
        if (idMarca) {
            var url = '/Articulo/GetArticulosPorMarca';
            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                data: { idMarca: idMarca },
                success: function (data) {
                    $('#articuloSelect').prop('disabled', false);
                    $articuloSelect.append('<option value="">Seleccione un artículo</option>');
                    $.each(data, function (index, articulo) {
                        $articuloSelect.append('<option value="' + articulo.idArticulo + '">' + articulo.nombre + '</option>');
                    });
                    $('#articuloSelect').trigger("chosen:updated");
                },
                error: function (xhr, status, error) {
                    console.log('Error: ' + error);
                }
            });
        } else {
            $('#articuloSelect').empty().append('<option value="">Seleccione un artículo</option>');
        }
    });

    $('#articuloSelect').on("change",function () {
        var $loteSelect = $('#loteSelect');
        var $marcaSelect = $('#marcaSelect');
        $loteSelect.empty();
        var idArticulo = $(this).val();
        if (idArticulo) {
            var url = '/Articulo/GetLotesPorArticulo';
            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                data: { idArticulo: idArticulo },
                success: function (data) {    
                    $('#loteSelect').prop('disabled', false);
                    $loteSelect.append('<option value=""> Ninguno </option>');
                    $.each(data.lotes, function (index, lote) {
                        $loteSelect.append('<option value="' +  lote.loteId + '">' + lote.numeroLote+ '</option>');
                    }); 
                    $marcaSelect.val(data.marca); 
                    $marcaSelect.trigger("chosen:updated");
                    $('#loteSelect').trigger("chosen:updated");

                    buscaCantidad();
                },
                error: function (xhr, status, error) {
                    console.log('Error: ' + error);
                    $('#loteSelect').prop('disabled', false);
                    $loteSelect.append('<option value=""> Ninguno </option>');
                    $('#loteSelect').trigger("chosen:updated");
                }
            });          
            
        } else {
            $('#loteSelect').empty().append('<option value="">Ninguno</option>');
        }
    });

    $('#ScopeId').on("change", function () {
        var $ScopeSelect = $('#ScopeId');
        var $DestinoSelect = $('#Destino');
        $("#rubroSelect").prop('disabled', false);
        $('#rubroSelect').trigger("chosen:updated");
        $("#cantidad-error").text("");
        $DestinoSelect.empty();
        var ScopeId = $(this).val();

        if (ScopeId > 0) {
            var url = '/Egresos/GetDestinos';
            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                data: { ScopeId: ScopeId },
                success: function (data) {       
                    $DestinoSelect.append('<option value=""> Seleccione un Destino </option>');
                    $.each(data, function (index, data) {
                        $DestinoSelect.append('<option value="' + data.scopeId + '">' + data.scopeName + '</option>');
                    }); 
                    $('#Destino').prop('disabled', false);
                    $DestinoSelect.trigger("chosen:updated");
                }               
            });
        } else {
            $('#Destino').empty().append('<option value="">Seleccione un Destino</option>');
            $('#Destino').prop('disabled', true);
            $DestinoSelect.trigger("chosen:updated");
        }
    });

    $('#loteSelect').on("change", function () {  
        buscaCantidad();
    });

    function buscaCantidad() {
        var $cantidad = $('#stock'); // Selector del campo de stock
        var idLote = $('#loteSelect').val(); // Obtener el valor seleccionado
        var idDeposito = $('#ScopeId').val();
        var idArticulo = $('#articuloSelect').val();

        var url = '/Articulo/GetCantidadPorLote';
        if (idDeposito != 'Seleccione un Deposito') {
            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'json',
                data: {
                    idLote: idLote,
                    idArticulo: idArticulo,
                    idDeposito: idDeposito
                },
                success: function (data) {
                    $cantidad.prop('disabled', false); // Habilitar el campo
                    $cantidad.val(data.cantidad); // Establecer el valor en el campo
                    if (data.cantidad == 0) {
                        $cantidad.prop("style", "color:red")
                    }
                    else {
                        $cantidad.prop("style", "color:green")
                    }
                },
                error: function (xhr, status, error) {
                    console.log('Error: ' + error);
                    $cantidad.prop('disabled', true); // Deshabilitar el campo en caso de error
                    $cantidad.val('Error al obtener cantidad'); // Mensaje de error
                }
            });
        }
        else {
            $('#cantidad-error').text('Seleccione Deposito');
        }
    }


    // Manejo del envío del formulario del modal para agregar un nuevo lote
    $('#createLoteForm').on("submit", function (event) {
        event.preventDefault(); // Evita el envío tradicional del formulario
        var url = '/Ingresos/CreateLote';
        $.ajax({            
            url: url,
            type: 'POST',
            data: $(this).serialize(),
            success: function (data) {
                if (data.success) {
                    // Actualiza la lista de lotes en el dropdown
                    $('#loteSelect').append(
                        $('<option>').val(data.loteId).text(data.numeroLote)
                    );

                    $('#loteSelect').val(data.loteId).trigger("chosen:updated");
                    // Cierra el modal
                    $('#createLoteModal').modal('hide');

                    $('#createLoteForm')[0].reset();

                    // Limpia cualquier mensaje de error
                    $('#numeroLoteError').text('');
                } else {
                    $('#numeroLoteError').text('Error al guardar el lote.');
                }
            },
            error: function () {
                $('#numeroLoteError').text('Error al comunicarse con el servidor.');
            }
        });
    });

});
