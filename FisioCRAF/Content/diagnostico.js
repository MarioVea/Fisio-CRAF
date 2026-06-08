'use strict';

/* 
   UTILIDADES
 */
const $ = id => document.getElementById(id);
const soloTexto = v => /^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ\s.,;:()\-']+$/.test(v.trim());
const soloNumeros = v => /^\d+$/.test(v.trim());

function mostrarAlerta(msg, tipo = 'error') {
    const div = document.createElement('div');
    div.textContent = msg;
    div.style.cssText = `
    position:fixed; top:80px; right:24px; z-index:9999;
    background:${tipo === 'ok' ? '#dcfce7' : '#fee2e2'};
    color:${tipo === 'ok' ? '#166534' : '#991b1b'};
    border:1px solid ${tipo === 'ok' ? '#bbf7d0' : '#fca5a5'};
    border-radius:8px; padding:12px 20px;
    font-family:'DM Sans',sans-serif; font-size:.88rem; font-weight:500;
    box-shadow:0 4px 12px rgba(0,0,0,.12);
    animation:fadeIn .2s ease;
  `;
    document.body.appendChild(div);
    setTimeout(() => div.remove(), 3200);
}

/* 
   ESTADO GLOBAL
 */
let diagnosticoSeleccionado = null; // objeto con datos del diag cargado
let filaEditando = null;
let zonaSeleccionada = null;
let vistaActual = 'anterior';

/* 
   TABS
 */
document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', () => {
        document.querySelectorAll('.tab').forEach(t => {
            t.classList.remove('active');
            t.setAttribute('aria-selected', 'false');
        });
        document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
        tab.classList.add('active');
        tab.setAttribute('aria-selected', 'true');
        $('tab-' + tab.dataset.tab).classList.add('active');
    });
});

/* 
   MODAL DE BÚSQUEDA (estilo Ejercicios)
 */
const modal = $('modalBusqueda');
const btnAbrirModal = $('abrirModal');
const btnCerrarModal = $('cerrarModal');
const btnCancelar = $('btnCancelar');
const btnAceptar = $('btnAceptar');
const modalInputBuscador = $('modalBuscadorInput');
const modalResultados = $('modalResultados');
let modalTimerId;
let itemSeleccionadoModal = null;

// Abrir modal
btnAbrirModal.addEventListener('click', function (e) {
    e.preventDefault();
    modal.removeAttribute('hidden');
    modalInputBuscador.value = '';
    modalResultados.innerHTML = '<tr class="empty-row"><td colspan="4">Ingrese un término de búsqueda.</td></tr>';
    itemSeleccionadoModal = null;
    modalInputBuscador.focus();
});

// Cerrar modal
function cerrarModalBusqueda() {
    modal.setAttribute('hidden', '');
}
btnCerrarModal.addEventListener('click', cerrarModalBusqueda);
btnCancelar.addEventListener('click', cerrarModalBusqueda);

// Buscar mientras escribes
modalInputBuscador.addEventListener('input', function () {
    clearTimeout(modalTimerId);
    const self = this;
    modalTimerId = setTimeout(function () {
        const valor = self.value.trim();
        if (!valor) {
            modalResultados.innerHTML = '<tr class="empty-row"><td colspan="4">Ingrese un término de búsqueda.</td></tr>';
            return;
        }

        fetch('/Diagnostico/BuscarDiagnosticos?busqueda=' + encodeURIComponent(valor))
            .then(function (response) { return response.json(); })
            .then(function (data) { mostrarResultadosModal(data); })
            .catch(function (error) { console.error("Error:", error); });
    }, 300);
});

function mostrarResultadosModal(resultados) {
    modalResultados.innerHTML = '';
    itemSeleccionadoModal = null;

    if (!resultados || !resultados.length) {
        modalResultados.innerHTML = '<tr class="empty-row"><td colspan="4">Sin resultados.</td></tr>';
        return;
    }

    resultados.forEach(function (r) {
        const fila = document.createElement('tr');
        fila.innerHTML =
            '<td>' + (r.Num_Diag > 0 ? r.Num_Diag : '<span style="color:var(--muted);font-size:.8rem">Nuevo</span>') + '</td>' +
            '<td>' + (r.Paciente || '') + '</td>' +
            '<td>' + (r.Motivo_Cita || '') + '</td>' +
            '<td>' + (r.Fecha_Cita || '') + '</td>';

        fila.addEventListener('click', function () {
            Array.from(modalResultados.querySelectorAll('tr')).forEach(function (row) { row.classList.remove('seleccionado'); });
            fila.classList.add('seleccionado');
            itemSeleccionadoModal = r;
        });

        modalResultados.appendChild(fila);
    });
}

// Aceptar selección
btnAceptar.addEventListener('click', function () {
    if (!itemSeleccionadoModal) {
        mostrarAlerta('Seleccione un registro de la tabla.');
        return;
    }

    // Cargar datos en los campos
    $('id-cita').value = itemSeleccionadoModal.id_Cita || '';
    $('id-diag').value = itemSeleccionadoModal.id_Diag || '';
    $('id-pac').value = itemSeleccionadoModal.id_Pac || '';
    $('id-emp').value = itemSeleccionadoModal.id_Emp || '';
    $('motivo-cita').value = itemSeleccionadoModal.Motivo_Cita || '';
    $('nombre-paciente').value = itemSeleccionadoModal.Paciente || '';
    $('nombre-fisio').value = itemSeleccionadoModal.Fisioterapeuta || '';
    $('num-diag').value = itemSeleccionadoModal.Num_Diag > 0 ? itemSeleccionadoModal.Num_Diag : '';

    if (itemSeleccionadoModal.id_Consul) {
        $('consultorio').value = itemSeleccionadoModal.id_Consul;
    }

    if (itemSeleccionadoModal.id_Diag > 0) {
        // Cargar detalles del diagnóstico existente
        fetch('/Diagnostico/ObtenerDetalleDiagnostico?id=' + itemSeleccionadoModal.id_Diag)
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.respuesta && result.datos) {
                    cargarDetallesEnVista(result.datos);
                }
            })
            .catch(function (error) { console.error("Error cargando detalles:", error); });

        // Bloquear guardar, habilitar actualizar y eliminar
        diagnosticoSeleccionado = itemSeleccionadoModal;
        $('btn-save').disabled = true;
        $('btn-save').style.opacity = '0.5';
        $('btn-actualizar').disabled = false;
        $('btn-actualizar').style.opacity = '1';
        $('btn-eliminar').disabled = false;
        $('btn-eliminar').style.opacity = '1';
    } else {
        // No tiene diagnóstico, preparar para guardar uno nuevo
        $('diag-tbody').innerHTML = '<tr class="empty-row"><td colspan="7">No hay registros. Presione ＋ para agregar.</td></tr>';
        $('ej-tbody').innerHTML = '<tr class="empty-row"><td colspan="5">No hay ejercicios asignados.</td></tr>';
        $('musculo-tbody').innerHTML = '<tr class="empty-row"><td colspan="4">Sin zonas registradas.</td></tr>';
        
        diagnosticoSeleccionado = null;
        $('btn-save').disabled = false;
        $('btn-save').style.opacity = '1';
        $('btn-actualizar').disabled = true;
        $('btn-actualizar').style.opacity = '0.5';
        $('btn-eliminar').disabled = true;
        $('btn-eliminar').style.opacity = '0.5';
    }

    cerrarModalBusqueda();
    
    if (itemSeleccionadoModal.id_Diag > 0) {
        mostrarAlerta('Diagnóstico #' + itemSeleccionadoModal.Num_Diag + ' cargado.', 'ok');
    } else {
        mostrarAlerta('Cita cargada. Listo para crear nuevo diagnóstico.', 'ok');
    }
});

function cargarDetallesEnVista(datos) {
    // Cargar consultorio
    if (datos.id_Consul) $('consultorio').value = datos.id_Consul;

    // Cargar DetallesDiag en la tabla
    var tbody = $('diag-tbody');
    tbody.innerHTML = '';
    if (datos.DetallesDiag && datos.DetallesDiag.length > 0) {
        datos.DetallesDiag.forEach(function (dd) {
            var tipoLabel = dd.Tipo_Diag ? 'Específico' : 'Genérico';
            var tr = document.createElement('tr');
            tr.innerHTML =
                '<td>' + tipoLabel + '</td>' +
                '<td>' + (dd.Nombre_Diag || '—') + '</td>' +
                '<td>' + (dd.Nombre_Lesion || '—') + '</td>' +
                '<td>' + (dd.RadioGrafia ? '<img src="' + dd.RadioGrafia + '" style="height:40px;border-radius:4px;" />' : '—') + '</td>' +
                '<td>' + (dd.Valor_Escala || '—') + '</td>' +
                '<td title="' + (dd.Descrip_Diag || '') + '">' + (dd.Descrip_Diag ? dd.Descrip_Diag.slice(0, 35) + (dd.Descrip_Diag.length > 35 ? '…' : '') : '—') + '</td>' +
                '<td>' +
                '<button class="btn-tbl btn-tbl-edit" title="Editar" onclick="abrirEditar(this)">✏️</button>' +
                '<button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaDiag(this)">🗑️</button>' +
                '</td>';
            tr.dataset.datos = JSON.stringify({
                tipo: dd.Tipo_Diag ? '1' : '0',
                nombre: dd.Nombre_Diag || '',
                id_Lesion: dd.id_Lesion || '',
                nombreLesion: dd.Nombre_Lesion || '',
                dolor: dd.Valor_Escala || '',
                id_EscalaDolor: dd.id_EscalaDolor || '',
                desc: dd.Descrip_Diag || '',
                radiografia: dd.RadioGrafia || ''
            });
            tbody.appendChild(tr);
        });
    } else {
        tbody.innerHTML = '<tr class="empty-row"><td colspan="7">No hay registros. Presione ＋ para agregar.</td></tr>';
    }

    // Cargar DetallesEjer en la tabla
    var ejTbody = $('ej-tbody');
    ejTbody.innerHTML = '';
    if (datos.DetallesEjer && datos.DetallesEjer.length > 0) {
        datos.DetallesEjer.forEach(function (de) {
            var tr = document.createElement('tr');
            tr.innerHTML =
                '<td>' + (de.Nombre_Ejer || '—') + '</td>' +
                '<td>' + (de.Nombre_CatEjer || '—') + '</td>' +
                '<td>' + de.Series + '</td>' +
                '<td>' + de.Repeticiones + '</td>' +
                '<td><button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaEj(this)">🗑️</button></td>';
            tr.dataset.datos = JSON.stringify({
                id_Ejercicio: de.id_Ejercicio,
                nombre: de.Nombre_Ejer,
                categoria: de.Nombre_CatEjer,
                series: de.Series,
                repeticiones: de.Repeticiones
            });
            ejTbody.appendChild(tr);
        });
    } else {
        ejTbody.innerHTML = '<tr class="empty-row"><td colspan="5">No hay ejercicios asignados.</td></tr>';
    }

    // Cargar DetallesCH en la tabla
    var musTbody = $('musculo-tbody');
    musTbody.innerHTML = '';
    if (datos.DetallesCH && datos.DetallesCH.length > 0) {
        datos.DetallesCH.forEach(function (ch) {
            var tr = document.createElement('tr');
            tr.innerHTML =
                '<td><strong>' + (ch.Nombre_Musculo || '—') + '</strong></td>' +
                '<td style="font-size:.8rem">' + (ch.Nombre_Trata || '—') + '</td>' +
                '<td title="' + (ch.Descripcion_DiagCH || '') + '">' + (ch.Descripcion_DiagCH ? ch.Descripcion_DiagCH.slice(0, 40) + (ch.Descripcion_DiagCH.length > 40 ? '…' : '') : '—') + '</td>' +
                '<td><button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaMusculo(this)">🗑️</button></td>';
            tr.dataset.datos = JSON.stringify({
                nombre: ch.Nombre_Musculo,
                imagen: ch.Imag_Musculo,
                id_Trata: ch.id_Trata,
                nombreTrata: ch.Nombre_Trata,
                desc: ch.Descripcion_DiagCH
            });
            musTbody.appendChild(tr);
        });
    } else {
        musTbody.innerHTML = '<tr class="empty-row"><td colspan="4">Sin zonas registradas.</td></tr>';
    }
}

/* 
   CERRAR MODALES
 */
function abrirModal(id) {
    var m = $(id);
    m.removeAttribute('hidden');
}
function cerrarModal(id) {
    $(id).setAttribute('hidden', '');
}
document.querySelectorAll('.modal-overlay').forEach(m => {
    m.addEventListener('click', e => { if (e.target === m) cerrarModal(m.id); });
});
document.addEventListener('keydown', e => {
    if (e.key !== 'Escape') return;
    ['modalBusqueda', 'modal-editar'].forEach(id => {
        if (!$(id).hasAttribute('hidden')) cerrarModal(id);
    });
});

/* 
   BOTÓN LIMPIAR
 */
$('btn-limpiar').addEventListener('click', function () {
    $('id-cita').value = '';
    $('id-diag').value = '';
    $('id-pac').value = '';
    $('id-emp').value = '';
    $('motivo-cita').value = '';
    $('nombre-paciente').value = '';
    $('nombre-fisio').value = '';
    $('num-diag').value = '';
    $('consultorio').value = '';

    // Limpiar tablas
    $('diag-tbody').innerHTML = '<tr class="empty-row"><td colspan="7">No hay registros. Presione ＋ para agregar.</td></tr>';
    $('ej-tbody').innerHTML = '<tr class="empty-row"><td colspan="5">No hay ejercicios asignados.</td></tr>';
    $('musculo-tbody').innerHTML = '<tr class="empty-row"><td colspan="4">Sin zonas registradas.</td></tr>';

    // Desbloquear guardar
    diagnosticoSeleccionado = null;
    $('btn-save').disabled = false;
    $('btn-save').style.opacity = '1';
    $('btn-actualizar').disabled = true;
    $('btn-actualizar').style.opacity = '0.5';
    $('btn-eliminar').disabled = true;
    $('btn-eliminar').style.opacity = '0.5';

    mostrarAlerta('Datos limpiados.', 'ok');
});

/* 
   RADIOGRAFÍA
 */
const xrayDrop = $('xray-drop');
const xrayFile = $('xray-file');
const xrayPreview = $('xray-preview');
const xrayLabel = $('xray-label');

xrayDrop.addEventListener('click', () => xrayFile.click());
xrayFile.addEventListener('change', () => {
    const file = xrayFile.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = ev => {
        xrayPreview.src = ev.target.result;
        xrayPreview.style.display = 'block';
        xrayLabel.style.display = 'none';
    };
    reader.readAsDataURL(file);
});

/* 
   TIPO DE LESIÓN → Cargar lesiones filtradas
 */
$('tipo-lesion').addEventListener('change', function () {
    var tipoId = this.value;
    var selLesion = $('sel-lesion');
    selLesion.innerHTML = '<option value="">— Seleccionar —</option>';

    if (!tipoId) return;

    var filtradas = (typeof LESIONES_DATA !== 'undefined' ? LESIONES_DATA : []).filter(function (l) {
        return String(l.id_TipoLes) === String(tipoId);
    });

    filtradas.forEach(function (l) {
        var opt = document.createElement('option');
        opt.value = l.id_Lesion;
        opt.textContent = l.Nom_les;
        selLesion.appendChild(opt);
    });
});

/* 
   TAB 1 — AGREGAR DIAGNÓSTICO A TABLA
 */
$('btn-add-diag').addEventListener('click', function () {
    var tipo = document.querySelector('input[name="tipoDiag"]:checked');
    var nombre = $('nombre-diag').value.trim();
    var lesionSel = $('sel-lesion');
    var lesionText = lesionSel.options[lesionSel.selectedIndex] ? lesionSel.options[lesionSel.selectedIndex].text : '';
    var lesionVal = lesionSel.value;
    var escalaSel = $('sel-escala');
    var escalaText = escalaSel.options[escalaSel.selectedIndex] ? escalaSel.options[escalaSel.selectedIndex].text : '';
    var escalaVal = escalaSel.value;
    var desc = $('desc-lesion').value.trim();
    var imgSrc = xrayPreview.src && xrayPreview.style.display !== 'none' ? xrayPreview.src : '';

    if (!nombre) { mostrarAlerta('Ingrese el nombre del diagnóstico.'); return; }
    if (!lesionVal) { mostrarAlerta('Seleccione una lesión.'); return; }
    if (!escalaVal) { mostrarAlerta('Seleccione la escala de dolor.'); return; }

    var tipoLabel = tipo && tipo.value === '1' ? 'Específico' : 'Genérico';
    var tbody = $('diag-tbody');
    var emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    var tr = document.createElement('tr');
    tr.innerHTML =
        '<td>' + tipoLabel + '</td>' +
        '<td>' + nombre + '</td>' +
        '<td>' + (lesionText || '—') + '</td>' +
        '<td>' + (imgSrc ? '<img src="' + imgSrc + '" style="height:40px;border-radius:4px;" />' : '—') + '</td>' +
        '<td>' + (escalaText || '—') + '</td>' +
        '<td title="' + desc + '">' + (desc ? desc.slice(0, 35) + (desc.length > 35 ? '…' : '') : '—') + '</td>' +
        '<td>' +
        '<button class="btn-tbl btn-tbl-edit" title="Editar" onclick="abrirEditar(this)">✏️</button>' +
        '<button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaDiag(this)">🗑️</button>' +
        '</td>';

    tr.dataset.datos = JSON.stringify({
        tipo: tipo ? tipo.value : '0',
        nombre: nombre,
        id_Lesion: lesionVal,
        nombreLesion: lesionText,
        dolor: escalaText,
        id_EscalaDolor: escalaVal,
        desc: desc,
        radiografia: imgSrc
    });
    tbody.appendChild(tr);

    // Limpiar campos
    $('nombre-diag').value = '';
    $('tipo-lesion').value = '';
    $('sel-lesion').innerHTML = '<option value="">— Seleccionar —</option>';
    $('sel-escala').value = '';
    $('desc-lesion').value = '';
    xrayPreview.style.display = 'none';
    xrayLabel.style.display = '';
    xrayPreview.src = '';
    xrayFile.value = '';

    mostrarAlerta('Registro agregado.', 'ok');
});

function quitarFilaDiag(btn) {
    btn.closest('tr').remove();
    if (!$('diag-tbody').querySelector('tr')) {
        $('diag-tbody').innerHTML = '<tr class="empty-row"><td colspan="7">No hay registros. Presione ＋ para agregar.</td></tr>';
    }
}

/* ── Editar fila ── */
function abrirEditar(btn) {
    filaEditando = btn.closest('tr');
    var d = JSON.parse(filaEditando.dataset.datos || '{}');
    $('edit-tipo').value = d.tipo || '0';
    $('edit-nombre').value = d.nombre || '';

    // Cargar lesiones en el modal edit
    var editLesion = $('edit-lesion');
    editLesion.innerHTML = '<option value="">— Seleccionar —</option>';
    (typeof LESIONES_DATA !== 'undefined' ? LESIONES_DATA : []).forEach(function (l) {
        var opt = document.createElement('option');
        opt.value = l.id_Lesion;
        opt.textContent = l.Nom_les;
        editLesion.appendChild(opt);
    });
    editLesion.value = d.id_Lesion || '';

    // Cargar escalas en el modal edit
    var editDolor = $('edit-dolor');
    editDolor.innerHTML = '<option value="">— Seleccionar —</option>';
    (typeof ESCALAS_DATA !== 'undefined' ? ESCALAS_DATA : []).forEach(function (e) {
        var opt = document.createElement('option');
        opt.value = e.id_EscalaDolor;
        opt.textContent = e.Valor_Escala;
        editDolor.appendChild(opt);
    });
    editDolor.value = d.id_EscalaDolor || '';

    $('edit-desc').value = d.desc || '';
    abrirModal('modal-editar');
}

$('btn-editar-guardar').addEventListener('click', function () {
    if (!filaEditando) return;
    var nombre = $('edit-nombre').value.trim();
    if (!nombre) { mostrarAlerta('El nombre no puede estar vacío.'); return; }

    var tipo = $('edit-tipo').value;
    var lesionSel = $('edit-lesion');
    var lesionVal = lesionSel.value;
    var lesionText = lesionSel.options[lesionSel.selectedIndex] ? lesionSel.options[lesionSel.selectedIndex].text : '';
    var dolorSel = $('edit-dolor');
    var dolorVal = dolorSel.value;
    var dolorText = dolorSel.options[dolorSel.selectedIndex] ? dolorSel.options[dolorSel.selectedIndex].text : '';
    var desc = $('edit-desc').value.trim();

    var oldData = JSON.parse(filaEditando.dataset.datos || '{}');
    filaEditando.dataset.datos = JSON.stringify({
        tipo: tipo,
        nombre: nombre,
        id_Lesion: lesionVal,
        nombreLesion: lesionText,
        dolor: dolorText,
        id_EscalaDolor: dolorVal,
        desc: desc,
        radiografia: oldData.radiografia || ''
    });
    var celdas = filaEditando.querySelectorAll('td');
    celdas[0].textContent = tipo === '1' ? 'Específico' : 'Genérico';
    celdas[1].textContent = nombre;
    celdas[2].textContent = lesionText || '—';
    celdas[4].textContent = dolorText || '—';
    celdas[5].textContent = desc ? desc.slice(0, 35) + (desc.length > 35 ? '…' : '') : '—';
    celdas[5].title = desc;

    cerrarModal('modal-editar');
    mostrarAlerta('Registro actualizado.', 'ok');
    filaEditando = null;
});

$('btn-editar-cancelar').addEventListener('click', () => cerrarModal('modal-editar'));
$('modal-editar-close').addEventListener('click', () => cerrarModal('modal-editar'));

/* 
   TAB 2 — EJERCICIOS
 */
let catActiva = 'todos';

function cargarEjercicios(cat) {
    var sel = $('sel-ejercicio');
    sel.innerHTML = '<option value="">— Seleccionar —</option>';
    var lista = (typeof EJERCICIOS_DATA !== 'undefined' ? EJERCICIOS_DATA : []);

    if (cat !== 'todos') {
        lista = lista.filter(function (e) { return String(e.id_CatEjer) === String(cat); });
    }
    lista.forEach(function (e) {
        var opt = document.createElement('option');
        opt.value = e.id_Ejercicio;
        opt.textContent = e.Nombre_Ejer;
        opt.dataset.cat = e.Nombre_CatEjer;
        opt.dataset.catId = e.id_CatEjer;
        sel.appendChild(opt);
    });
}

document.querySelectorAll('.cat-pill').forEach(pill => {
    pill.addEventListener('click', () => {
        document.querySelectorAll('.cat-pill').forEach(p => p.classList.remove('active'));
        pill.classList.add('active');
        catActiva = pill.dataset.cat;
        cargarEjercicios(catActiva);
    });
});
cargarEjercicios('todos');

$('btn-add-ej').addEventListener('click', function () {
    var selEj = $('sel-ejercicio');
    var series = $('ej-series').value.trim();
    var reps = $('ej-repeticiones').value.trim();

    if (!selEj.value) { mostrarAlerta('Seleccione un ejercicio.'); return; }
    if (!series || !soloNumeros(series) || parseInt(series) < 1) { mostrarAlerta('Ingrese series válidas (mínimo 1).'); return; }
    if (!reps || !soloNumeros(reps) || parseInt(reps) < 1) { mostrarAlerta('Ingrese repeticiones válidas (mínimo 1).'); return; }

    var opt = selEj.options[selEj.selectedIndex];
    var tbody = $('ej-tbody');
    var emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    var tr = document.createElement('tr');
    tr.innerHTML =
        '<td>' + opt.textContent + '</td>' +
        '<td>' + (opt.dataset.cat || '—') + '</td>' +
        '<td>' + parseInt(series) + '</td>' +
        '<td>' + parseInt(reps) + '</td>' +
        '<td><button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaEj(this)">🗑️</button></td>';
    tr.dataset.datos = JSON.stringify({
        id_Ejercicio: parseInt(selEj.value),
        nombre: opt.textContent,
        categoria: opt.dataset.cat || '',
        series: parseInt(series),
        repeticiones: parseInt(reps)
    });
    tbody.appendChild(tr);

    selEj.value = '';
    $('ej-series').value = '';
    $('ej-repeticiones').value = '';
    mostrarAlerta('Ejercicio asignado.', 'ok');
});

function quitarFilaEj(btn) {
    btn.closest('tr').remove();
    if (!$('ej-tbody').querySelector('tr')) {
        $('ej-tbody').innerHTML = '<tr class="empty-row"><td colspan="5">No hay ejercicios asignados.</td></tr>';
    }
}

/* 
   TAB 3 — SELECCIÓN DE MÚSCULO (SVG)
 */

/* ZONAS MUSCULARES (Aproximaciones sobre la imagen cuerpo_humano.png) */
const ZONAS = {
    anterior: [
        { id: 'deltoides-izq-ant', nombre: 'Deltoides Izquierdo', tag: '<ellipse cx="58" cy="135" rx="15" ry="30" transform="rotate(15 58 135)"' },
        { id: 'deltoides-der-ant', nombre: 'Deltoides Derecho', tag: '<ellipse cx="178" cy="135" rx="15" ry="30" transform="rotate(-15 178 135)"' },
        { id: 'pectoral-izq', nombre: 'Pectoral Izquierdo', tag: '<ellipse cx="90" cy="135" rx="25" ry="20"' },
        { id: 'pectoral-der', nombre: 'Pectoral Derecho', tag: '<ellipse cx="145" cy="135" rx="25" ry="20"' },
        { id: 'biceps-izq', nombre: 'Bíceps Izquierdo', tag: '<ellipse cx="45" cy="190" rx="12" ry="25" transform="rotate(20 45 190)"' },
        { id: 'biceps-der', nombre: 'Bíceps Derecho', tag: '<ellipse cx="190" cy="190" rx="12" ry="25" transform="rotate(-20 190 190)"' },
        { id: 'antebrazo-izq-ant', nombre: 'Antebrazo Izquierdo', tag: '<ellipse cx="32" cy="240" rx="10" ry="25" transform="rotate(25 32 240)"' },
        { id: 'antebrazo-der-ant', nombre: 'Antebrazo Derecho', tag: '<ellipse cx="203" cy="240" rx="10" ry="25" transform="rotate(-25 203 240)"' },
        { id: 'abdominales', nombre: 'Abdominales', tag: '<rect x="95" y="160" width="45" height="80" rx="10"' },
        { id: 'oblicuo-izq', nombre: 'Oblicuo Izquierdo', tag: '<ellipse cx="80" cy="190" rx="10" ry="35" transform="rotate(10 80 190)"' },
        { id: 'oblicuo-der', nombre: 'Oblicuo Derecho', tag: '<ellipse cx="155" cy="190" rx="10" ry="35" transform="rotate(-10 155 190)"' },
        { id: 'cuadriceps-izq', nombre: 'Cuádriceps Izquierdo', tag: '<ellipse cx="85" cy="285" rx="20" ry="45"' },
        { id: 'cuadriceps-der', nombre: 'Cuádriceps Derecho', tag: '<ellipse cx="150" cy="285" rx="20" ry="45"' },
        { id: 'tibial-izq', nombre: 'Tibial Anterior Izquierdo', tag: '<ellipse cx="85" cy="385" rx="12" ry="40"' },
        { id: 'tibial-der', nombre: 'Tibial Anterior Derecho', tag: '<ellipse cx="150" cy="385" rx="12" ry="40"' },
    ],
    posterior: [
        { id: 'trapecio', nombre: 'Trapecio', tag: '<polygon points="350,90 325,120 375,120"' },
        { id: 'deltoides-izq-post', nombre: 'Deltoides Post. Izquierdo', tag: '<ellipse cx="290" cy="135" rx="15" ry="30" transform="rotate(15 290 135)"' },
        { id: 'deltoides-der-post', nombre: 'Deltoides Post. Derecho', tag: '<ellipse cx="410" cy="135" rx="15" ry="30" transform="rotate(-15 410 135)"' },
        { id: 'espalda-alta-izq', nombre: 'Espalda Alta Izquierda', tag: '<polygon points="350,120 310,130 315,180 350,210"' },
        { id: 'espalda-alta-der', nombre: 'Espalda Alta Derecha', tag: '<polygon points="350,120 390,130 385,180 350,210"' },
        { id: 'triceps-izq', nombre: 'Tríceps Izquierdo', tag: '<ellipse cx="280" cy="190" rx="12" ry="25" transform="rotate(20 280 190)"' },
        { id: 'triceps-der', nombre: 'Tríceps Derecho', tag: '<ellipse cx="420" cy="190" rx="12" ry="25" transform="rotate(-20 420 190)"' },
        { id: 'antebrazo-izq-post', nombre: 'Antebrazo Post. Izquierdo', tag: '<ellipse cx="265" cy="240" rx="10" ry="25" transform="rotate(25 265 240)"' },
        { id: 'antebrazo-der-post', nombre: 'Antebrazo Post. Derecho', tag: '<ellipse cx="435" cy="240" rx="10" ry="25" transform="rotate(-25 435 240)"' },
        { id: 'lumbar', nombre: 'Zona Lumbar', tag: '<polygon points="350,210 320,240 380,240"' },
        { id: 'gluteo-izq', nombre: 'Glúteo Izquierdo', tag: '<ellipse cx="320" cy="255" rx="25" ry="20"' },
        { id: 'gluteo-der', nombre: 'Glúteo Derecho', tag: '<ellipse cx="380" cy="255" rx="25" ry="20"' },
        { id: 'isquiotibial-izq', nombre: 'Isquiotibial Izquierdo', tag: '<ellipse cx="315" cy="315" rx="18" ry="40"' },
        { id: 'isquiotibial-der', nombre: 'Isquiotibial Derecho', tag: '<ellipse cx="385" cy="315" rx="18" ry="40"' },
        { id: 'gemelo-izq', nombre: 'Gemelo Izquierdo', tag: '<ellipse cx="315" cy="390" rx="15" ry="35"' },
        { id: 'gemelo-der', nombre: 'Gemelo Derecho', tag: '<ellipse cx="385" cy="390" rx="15" ry="35"' },
    ]
};

function generarSVG(vista) {
    var zonas = ZONAS[vista];
    var xOffset = vista === 'anterior' ? 0 : 233;

    var zonasSVG = zonas.map(function (z) {
        return z.tag + ' class="zona-muscular" data-zona="' + z.id + '" data-nombre="' + z.nombre + '" ' +
            'fill="transparent" stroke="transparent" stroke-width="0" ' +
            'role="button" tabindex="0" aria-label="' + z.nombre + '" style="cursor:pointer; transition: fill 0.2s;"/>';
    }).join('\n');

    return '<svg viewBox="' + xOffset + ' 0 233 481" xmlns="http://www.w3.org/2000/svg" aria-label="Cuerpo humano vista ' + vista + '" style="width:100%; height:100%; border-radius:12px;">' +
        '<image href="/Content/cuerpo_humano.png" x="0" y="0" width="467" height="481" />' +
        zonasSVG +
        '</svg>';
}

function renderBody() {
    var canvas = $('body-canvas');
    canvas.innerHTML = generarSVG(vistaActual);
    canvas.querySelectorAll('.zona-muscular').forEach(function (zona) {
        zona.addEventListener('click', function () { seleccionarZona(zona); });
        zona.addEventListener('mouseenter', function () { 
            if (!zona.classList.contains('selected')) zona.setAttribute('fill', 'rgba(255,255,255,0.4)');
        });
        zona.addEventListener('mouseleave', function () {
            if (!zona.classList.contains('selected')) zona.setAttribute('fill', 'transparent');
        });
    });
}

function seleccionarZona(zona) {
    $('body-canvas').querySelectorAll('.zona-muscular').forEach(function (z) {
        z.classList.remove('selected');
        z.setAttribute('fill', 'transparent');
    });
    zona.classList.add('selected');
    zona.setAttribute('fill', 'rgba(20, 184, 166, 0.5)'); // Teal highlight
    zonaSeleccionada = { id: zona.dataset.zona, nombre: zona.dataset.nombre };
    $('zona-sel-display').value = zonaSeleccionada.nombre;
    $('zona-nombre-texto').textContent = zonaSeleccionada.nombre;
}

document.querySelectorAll('.vista-btn').forEach(function (btn) {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.vista-btn').forEach(function (b) { b.classList.remove('active'); });
        btn.classList.add('active');
        vistaActual = btn.dataset.vista;
        zonaSeleccionada = null;
        $('zona-sel-display').value = '';
        $('zona-nombre-texto').textContent = 'Ninguna zona seleccionada';
        renderBody();
    });
});

/* Agregar zona a tabla */
$('btn-add-musculo').addEventListener('click', function () {
    if (!zonaSeleccionada) { mostrarAlerta('Seleccione una zona en el cuerpo.'); return; }

    var tratSel = $('tratamiento-select');
    var tratVal = tratSel.value;
    var tratText = tratSel.options[tratSel.selectedIndex] ? tratSel.options[tratSel.selectedIndex].text : '';
    var desc = $('musculo-desc').value.trim();

    var tbody = $('musculo-tbody');
    var emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    var tr = document.createElement('tr');
    tr.innerHTML =
        '<td><strong>' + zonaSeleccionada.nombre + '</strong><br/><small style="color:var(--muted);text-transform:capitalize">' + vistaActual + '</small></td>' +
        '<td style="font-size:.8rem">' + (tratText || '—') + '</td>' +
        '<td title="' + desc + '">' + (desc ? desc.slice(0, 40) + (desc.length > 40 ? '…' : '') : '—') + '</td>' +
        '<td><button class="btn-tbl btn-tbl-del" title="Quitar" onclick="quitarFilaMusculo(this)">🗑️</button></td>';
    tr.dataset.datos = JSON.stringify({
        nombre: zonaSeleccionada.nombre,
        imagen: zonaSeleccionada.id,
        id_Trata: tratVal ? parseInt(tratVal) : null,
        nombreTrata: tratText,
        desc: desc
    });
    tbody.appendChild(tr);

    $('zona-sel-display').value = '';
    $('zona-nombre-texto').textContent = 'Ninguna zona seleccionada';
    $('musculo-desc').value = '';
    $('tratamiento-select').value = '';
    $('body-canvas').querySelectorAll('.zona-muscular').forEach(function (z) {
        z.classList.remove('selected');
        z.style.opacity = '.85';
        z.style.filter = '';
    });
    zonaSeleccionada = null;
    mostrarAlerta('Zona muscular registrada.', 'ok');
});

function quitarFilaMusculo(btn) {
    btn.closest('tr').remove();
    if (!$('musculo-tbody').querySelector('tr')) {
        $('musculo-tbody').innerHTML = '<tr class="empty-row"><td colspan="4">Sin zonas registradas.</td></tr>';
    }
}

renderBody();

/* 
   GUARDAR DIAGNÓSTICO
 */
$('btn-save').addEventListener('click', function () {
    var idCita = $('id-cita').value;
    if (!idCita) {
        mostrarAlerta('Primero busque y seleccione una cita/paciente.');
        return;
    }

    var payload = recopilarDatos();
    payload.id_Cita = parseInt(idCita);

    fetch('/Diagnostico/GuardarDiagnostico', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
    .then(function (response) { return response.json(); })
    .then(function (data) {
        if (data.respuesta) {
            mostrarAlerta('Diagnóstico guardado exitosamente.', 'ok');
            $('btn-limpiar').click();
        } else {
            mostrarAlerta('Error: ' + (data.mensaje || 'No se pudo guardar.'));
        }
    })
    .catch(function (error) {
        console.error("Error:", error);
        mostrarAlerta('Ocurrió un error al guardar.');
    });
});

/* 
   ACTUALIZAR DIAGNÓSTICO
 */
$('btn-actualizar').addEventListener('click', function () {
    if (!diagnosticoSeleccionado || !diagnosticoSeleccionado.id_Diag) {
        mostrarAlerta('Primero busque y seleccione un diagnóstico para actualizar.');
        return;
    }

    var payload = recopilarDatos();
    payload.id_Diag = parseInt(diagnosticoSeleccionado.id_Diag);

    fetch('/Diagnostico/ActualizarDiagnostico', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
    .then(function (response) { return response.json(); })
    .then(function (data) {
        if (data.respuesta) {
            mostrarAlerta('Diagnóstico actualizado exitosamente.', 'ok');
        } else {
            mostrarAlerta('Error: ' + (data.mensaje || 'No se pudo actualizar.'));
        }
    })
    .catch(function (error) {
        console.error("Error:", error);
        mostrarAlerta('Ocurrió un error al actualizar.');
    });
});

/* 
   ELIMINAR DIAGNÓSTICO
 */
$('btn-eliminar').addEventListener('click', function () {
    if (!diagnosticoSeleccionado || !diagnosticoSeleccionado.id_Diag) {
        mostrarAlerta('Primero busque y seleccione un diagnóstico para eliminar.');
        return;
    }

    if (!confirm('¿Está seguro de eliminar el diagnóstico #' + diagnosticoSeleccionado.Num_Diag + '? Esta acción no se puede deshacer.')) {
        return;
    }

    var formData = new FormData();
    formData.append('id_Diag', diagnosticoSeleccionado.id_Diag);

    fetch('/Diagnostico/EliminarDiagnostico', {
        method: 'POST',
        body: formData
    })
    .then(function (response) { return response.json(); })
    .then(function (data) {
        if (data.respuesta) {
            mostrarAlerta('Diagnóstico eliminado exitosamente.', 'ok');
            $('btn-limpiar').click();
        } else {
            mostrarAlerta('Error: ' + (data.mensaje || 'No se pudo eliminar.'));
        }
    })
    .catch(function (error) {
        console.error("Error:", error);
        mostrarAlerta('Ocurrió un error al eliminar.');
    });
});

/* 
   RECOPILAR DATOS DE LAS TABLAS
 */
function recopilarDatos() {
    var consulVal = $('consultorio').value;

    // Recopilar DetallesDiag
    var detallesDiag = [];
    $('diag-tbody').querySelectorAll('tr:not(.empty-row)').forEach(function (tr) {
        var d = JSON.parse(tr.dataset.datos || '{}');
        detallesDiag.push({
            Tipo_Diag: d.tipo === '1',
            Nombre_Diag: d.nombre || '',
            id_Lesion: d.id_Lesion ? parseInt(d.id_Lesion) : null,
            RadioGrafia: d.radiografia || null,
            Descrip_Diag: d.desc || '',
            id_EscalaDolor: d.id_EscalaDolor ? parseInt(d.id_EscalaDolor) : null
        });
    });

    // Recopilar DetallesEjer
    var detallesEjer = [];
    $('ej-tbody').querySelectorAll('tr:not(.empty-row)').forEach(function (tr) {
        var d = JSON.parse(tr.dataset.datos || '{}');
        detallesEjer.push({
            id_Ejercicio: d.id_Ejercicio || null,
            id_DetalleEjerLes: null,
            Series: d.series || 1,
            Repeticiones: d.repeticiones || 1
        });
    });

    // Recopilar DetallesCH
    var detallesCH = [];
    $('musculo-tbody').querySelectorAll('tr:not(.empty-row)').forEach(function (tr) {
        var d = JSON.parse(tr.dataset.datos || '{}');
        detallesCH.push({
            Imag_Musculo: d.imagen || '',
            Nombre_Musculo: d.nombre || '',
            Descripcion_DiagCH: d.desc || '',
            id_Trata: d.id_Trata || null
        });
    });

    return {
        id_Consul: consulVal ? parseInt(consulVal) : null,
        DetallesDiag: detallesDiag,
        DetallesEjer: detallesEjer,
        DetallesCH: detallesCH
    };
}

/* Exponer funciones globales para onclick inline */
window.abrirEditar = abrirEditar;
window.quitarFilaDiag = quitarFilaDiag;
window.quitarFilaEj = quitarFilaEj;
window.quitarFilaMusculo = quitarFilaMusculo;
