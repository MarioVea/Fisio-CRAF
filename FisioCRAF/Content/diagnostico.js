'use strict';

/* ═══════════════════════════════════════════
   DATOS SIMULADOS (reemplazar con BD real)
════════════════════════════════════════════ */
const PACIENTES_DB = [
    { id: 1, nombre: 'García López, María', motivo: 'Dolor lumbar crónico', fisio: 'Dra. Sánchez Torres' },
    { id: 2, nombre: 'Martínez Ruiz, Carlos', motivo: 'Rehabilitación post-operatoria', fisio: 'Dr. Ramírez Vega' },
    { id: 3, nombre: 'Hernández Mora, Ana', motivo: 'Esguince de tobillo', fisio: 'Dra. Sánchez Torres' },
    { id: 4, nombre: 'López Castro, Pedro', motivo: 'Cervicalgia', fisio: 'Dr. Ramírez Vega' },
    { id: 5, nombre: 'Rodríguez Díaz, Laura', motivo: 'Tendinitis rotuliana', fisio: 'Dra. Flores Mendoza' },
    { id: 6, nombre: 'Torres Guzmán, Jorge', motivo: 'Dolor de hombro', fisio: 'Dr. Ramírez Vega' },
];

const EJERCICIOS_DB = [
    { id: 1, nombre: 'Extensión de rodilla', cat: 'fuerza' },
    { id: 2, nombre: 'Flexión de cadera', cat: 'movilidad' },
    { id: 3, nombre: 'Puente glúteo', cat: 'fuerza' },
    { id: 4, nombre: 'Estiramiento isquiotibial', cat: 'estiramiento' },
    { id: 5, nombre: 'Equilibrio monopodal', cat: 'equilibrio' },
    { id: 6, nombre: 'Respiración diafragmática', cat: 'respiratorio' },
    { id: 7, nombre: 'Rotación de hombro', cat: 'movilidad' },
    { id: 8, nombre: 'Press de pecho con banda', cat: 'fuerza' },
    { id: 9, nombre: 'Estiramiento de gemelos', cat: 'estiramiento' },
    { id: 10, nombre: 'Marcha en puntillas', cat: 'equilibrio' },
    { id: 11, nombre: 'Elevación de pierna recta', cat: 'fuerza' },
    { id: 12, nombre: 'Respiración costal', cat: 'respiratorio' },
];

/* Registros simulados para el modal de eliminar */
let REGISTROS_DB = [
    { num: 1, paciente: 'García López, María', motivo: 'Dolor lumbar crónico' },
    { num: 2, paciente: 'Martínez Ruiz, Carlos', motivo: 'Rehabilitación post-operatoria' },
    { num: 3, paciente: 'Hernández Mora, Ana', motivo: 'Esguince de tobillo' },
];

/* Contador de Núm. Diag */
let numDiagActual = 1;

/* Fila en edición */
let filaEditando = null;
/* Registro a eliminar (modal) */
let registroAEliminar = null;

/* ═══════════════════════════════════════════
   UTILIDADES
════════════════════════════════════════════ */
const $ = id => document.getElementById(id);
const soloTexto = v => /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s.,'-]+$/.test(v.trim());
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

/* ═══════════════════════════════════════════
   TABS
════════════════════════════════════════════ */
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

/* ═══════════════════════════════════════════
   BÚSQUEDA GLOBAL — Autocomplete
════════════════════════════════════════════ */
const searchInput = $('search-global');
const suggestions = $('search-suggestions');

const campoMotivo = $('motivo-cita');
const campoPaciente = $('nombre-paciente');
const campoFisio = $('nombre-fisio');

searchInput.addEventListener('input', () => {
    const q = searchInput.value.trim().toLowerCase();
    suggestions.innerHTML = '';

    if (q.length < 2) { suggestions.classList.remove('open'); return; }

    const filtrados = PACIENTES_DB.filter(p =>
        p.nombre.toLowerCase().includes(q) || p.motivo.toLowerCase().includes(q)
    );

    if (!filtrados.length) {
        suggestions.classList.remove('open');
        return;
    }

    filtrados.forEach(p => {
        const li = document.createElement('li');
        li.textContent = `${p.nombre} — ${p.motivo}`;
        li.setAttribute('role', 'option');
        li.addEventListener('click', () => seleccionarPaciente(p));
        suggestions.appendChild(li);
    });

    suggestions.classList.add('open');
});

document.addEventListener('click', e => {
    if (!e.target.closest('.search-wrap')) suggestions.classList.remove('open');
});

function seleccionarPaciente(p) {
    campoMotivo.value = p.motivo;
    campoPaciente.value = p.nombre;
    campoFisio.value = p.fisio;

    [campoMotivo, campoPaciente, campoFisio].forEach(c => {
        c.readOnly = true;
        c.classList.add('locked');
    });

    searchInput.value = p.nombre;
    suggestions.classList.remove('open');
    mostrarAlerta(`Paciente "${p.nombre}" seleccionado.`, 'ok');
}

/* ═══════════════════════════════════════════
   BOTÓN LIMPIAR DATOS
════════════════════════════════════════════ */
$('btn-limpiar').addEventListener('click', () => {
    [campoMotivo, campoPaciente, campoFisio].forEach(c => {
        c.value = '';
        c.readOnly = false;
        c.classList.remove('locked');
    });
    searchInput.value = '';
    suggestions.classList.remove('open');
    mostrarAlerta('Campos de paciente limpiados.', 'ok');
});

/* ═══════════════════════════════════════════
   NÚM. DIAG — solo positivo, consecutivo, bloqueado
════════════════════════════════════════════ */
const inputNumDiag = $('num-diag');
inputNumDiag.value = numDiagActual;
inputNumDiag.setAttribute('readonly', true);
inputNumDiag.setAttribute('min', '1');

/* ═══════════════════════════════════════════
   RADIOGRAFÍA
════════════════════════════════════════════ */
const xrayDrop = $('xray-drop');
const xrayFile = $('xray-file');
const xrayPreview = $('xray-preview');
const xrayLabel = $('xray-label');

xrayDrop.addEventListener('click', () => xrayFile.click());
xrayDrop.addEventListener('keydown', e => { if (e.key === 'Enter' || e.key === ' ') xrayFile.click(); });

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

/* ═══════════════════════════════════════════
   TAB 1 — TABLA DIAGNÓSTICOS
════════════════════════════════════════════ */
const dolorLabels = ['Sin Dolor', 'Leve', 'Moderado', 'Fuerte', 'Extremo'];

$('btn-add-diag').addEventListener('click', () => {
    // Validaciones
    const tipo = document.querySelector('input[name="tipoDiag"]:checked')?.value || '';
    const nombre = $('nombre-diag').value.trim();
    const lesion = $('sel-lesion').options[$('sel-lesion').selectedIndex]?.text || '';
    const dolorEl = document.querySelector('input[name="dolor"]:checked');
    const desc = $('desc-lesion').value.trim();

    if (!nombre) { mostrarAlerta('Ingrese el nombre del diagnóstico.'); return; }
    if (!soloTexto(nombre)) { mostrarAlerta('El nombre del diagnóstico solo admite texto.'); return; }
    if (!$('sel-lesion').value) { mostrarAlerta('Seleccione una lesión.'); return; }
    if (!dolorEl) { mostrarAlerta('Seleccione la escala de dolor.'); return; }

    const dolorVal = dolorLabels[parseInt(dolorEl.value)];
    const tipoLabel = tipo === 'generico' ? 'Genérico' : 'Específico';
    const imgSrc = xrayPreview.src && xrayPreview.style.display !== 'none' ? xrayPreview.src : null;

    const tbody = $('diag-tbody');
    const emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    const tr = document.createElement('tr');
    tr.dataset.idx = numDiagActual;
    tr.innerHTML = `
    <td>${tipoLabel}</td>
    <td>${nombre}</td>
    <td>${lesion}</td>
    <td>${imgSrc ? `<img src="${imgSrc}" alt="Radiografía" style="height:40px;border-radius:4px;cursor:pointer;" onclick="window.open('${imgSrc}')"/>` : '—'}</td>
    <td>${dolorVal}</td>
    <td title="${desc}">${desc ? desc.slice(0, 35) + (desc.length > 35 ? '…' : '') : '—'}</td>
    <td>
      <button class="btn-tbl btn-tbl-edit" title="Editar registro" onclick="abrirEditar(this)">✏️</button>
      <button class="btn-tbl btn-tbl-del"  title="Quitar registro" onclick="quitarFilaDiag(this)">🗑️</button>
    </td>
  `;
    tr.dataset.datos = JSON.stringify({ tipo, nombre, lesion, dolor: dolorVal, desc });
    tbody.appendChild(tr);

    numDiagActual++;
    inputNumDiag.value = numDiagActual;

    // Limpiar formulario
    $('nombre-diag').value = '';
    $('tipo-lesion').value = '';
    $('sel-lesion').value = '';
    $('desc-lesion').value = '';
    document.querySelectorAll('input[name="dolor"]').forEach(r => r.checked = false);
    xrayPreview.style.display = 'none';
    xrayLabel.style.display = '';
    xrayPreview.src = '';
    xrayFile.value = '';

    mostrarAlerta('Registro agregado correctamente.', 'ok');
});

function quitarFilaDiag(btn) {
    const tr = btn.closest('tr');
    tr.remove();
    if (!$('diag-tbody').querySelector('tr')) {
        $('diag-tbody').innerHTML = '<tr class="empty-row"><td colspan="7">No hay registros. Presione ＋ para agregar.</td></tr>';
    }
}

/* ── Editar fila ── */
function abrirEditar(btn) {
    filaEditando = btn.closest('tr');
    const d = JSON.parse(filaEditando.dataset.datos || '{}');
    $('edit-tipo').value = d.tipo || 'generico';
    $('edit-nombre').value = d.nombre || '';
    $('edit-lesion').value = d.lesion || '';
    $('edit-dolor').value = d.dolor || 'Sin Dolor';
    $('edit-desc').value = d.desc || '';
    abrirModal('modal-editar');
}

$('btn-editar-guardar').addEventListener('click', () => {
    if (!filaEditando) return;
    const nombre = $('edit-nombre').value.trim();
    if (!nombre) { mostrarAlerta('El nombre no puede estar vacío.'); return; }
    if (!soloTexto(nombre)) { mostrarAlerta('El nombre solo admite texto.'); return; }

    const tipo = $('edit-tipo').value;
    const lesion = $('edit-lesion').value.trim();
    const dolor = $('edit-dolor').value;
    const desc = $('edit-desc').value.trim();

    filaEditando.dataset.datos = JSON.stringify({ tipo, nombre, lesion, dolor, desc });
    const celdas = filaEditando.querySelectorAll('td');
    celdas[0].textContent = tipo === 'generico' ? 'Genérico' : 'Específico';
    celdas[1].textContent = nombre;
    celdas[2].textContent = lesion || '—';
    celdas[4].textContent = dolor;
    celdas[5].textContent = desc ? desc.slice(0, 35) + (desc.length > 35 ? '…' : '') : '—';
    celdas[5].title = desc;

    cerrarModal('modal-editar');
    mostrarAlerta('Registro actualizado.', 'ok');
    filaEditando = null;
});

$('btn-editar-cancelar').addEventListener('click', () => cerrarModal('modal-editar'));
$('modal-editar-close').addEventListener('click', () => cerrarModal('modal-editar'));

/* ═══════════════════════════════════════════
   MODAL ELIMINAR REGISTRO
════════════════════════════════════════════ */
$('btn-eliminar').addEventListener('click', () => {
    renderTablaModal('');
    $('modal-search-input').value = '';
    abrirModal('modal-eliminar');
});

$('modal-eliminar-close').addEventListener('click', () => cerrarModal('modal-eliminar'));

$('modal-search-input').addEventListener('input', () => {
    renderTablaModal($('modal-search-input').value.trim().toLowerCase());
});

function renderTablaModal(q) {
    const tbody = $('modal-resultados-tbody');
    tbody.innerHTML = '';

    const lista = q.length
        ? REGISTROS_DB.filter(r =>
            r.paciente.toLowerCase().includes(q) || r.motivo.toLowerCase().includes(q))
        : REGISTROS_DB;

    if (!lista.length) {
        tbody.innerHTML = '<tr class="empty-row"><td colspan="4">Sin resultados.</td></tr>';
        return;
    }

    lista.forEach(r => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
      <td>${r.num}</td>
      <td>${r.paciente}</td>
      <td>${r.motivo}</td>
      <td>
        <button class="btn-add" style="width:auto;padding:4px 14px;font-size:.8rem;"
          onclick="pedirConfirmacionEliminar(${r.num})">Seleccionar</button>
      </td>
    `;
        tbody.appendChild(tr);
    });
}

function pedirConfirmacionEliminar(num) {
    registroAEliminar = REGISTROS_DB.find(r => r.num === num);
    if (!registroAEliminar) return;
    $('modal-confirmar-msg').textContent =
        `Estás a punto de eliminar el registro #${registroAEliminar.num} del paciente "${registroAEliminar.paciente}" con motivo "${registroAEliminar.motivo}". Esta acción no se puede deshacer.`;
    cerrarModal('modal-eliminar');
    abrirModal('modal-confirmar');
}

$('btn-confirmar-cancelar').addEventListener('click', () => {
    registroAEliminar = null;
    cerrarModal('modal-confirmar');
});

$('btn-confirmar-eliminar').addEventListener('click', () => {
    if (!registroAEliminar) return;
    REGISTROS_DB = REGISTROS_DB.filter(r => r.num !== registroAEliminar.num);
    mostrarAlerta(`Registro #${registroAEliminar.num} eliminado.`, 'ok');
    registroAEliminar = null;
    cerrarModal('modal-confirmar');
});

/* ── Helpers modales ── */
function abrirModal(id) {
    const m = $(id);
    m.removeAttribute('hidden');
    m.querySelector('button,input')?.focus();
}
function cerrarModal(id) {
    $(id).setAttribute('hidden', '');
}

// Cerrar modal al click fuera
document.querySelectorAll('.modal-overlay').forEach(m => {
    m.addEventListener('click', e => { if (e.target === m) cerrarModal(m.id); });
});

// Cerrar con Escape
document.addEventListener('keydown', e => {
    if (e.key !== 'Escape') return;
    ['modal-eliminar', 'modal-confirmar', 'modal-editar'].forEach(id => {
        if (!$(id).hasAttribute('hidden')) cerrarModal(id);
    });
});

/* ═══════════════════════════════════════════
   BOTÓN GUARDAR DATOS
════════════════════════════════════════════ */
$('btn-save').addEventListener('click', () => {
    const motivo = $('motivo-cita').value.trim();
    const paciente = $('nombre-paciente').value.trim();
    const fisio = $('nombre-fisio').value.trim();

    if (!motivo || !paciente || !fisio) {
        mostrarAlerta('Complete los campos de Motivo, Paciente y Fisioterapeuta.');
        return;
    }
    // Aquí iría la llamada al backend
    mostrarAlerta('Datos guardados correctamente.', 'ok');
});

/* ═══════════════════════════════════════════
   TAB 2 — EJERCICIOS
════════════════════════════════════════════ */
let catActiva = 'todos';

function cargarEjercicios(cat) {
    const sel = $('sel-ejercicio');
    sel.innerHTML = '<option value="">— Seleccionar —</option>';
    const lista = cat === 'todos'
        ? EJERCICIOS_DB
        : EJERCICIOS_DB.filter(e => e.cat === cat);
    lista.forEach(e => {
        const opt = document.createElement('option');
        opt.value = e.id;
        opt.textContent = e.nombre;
        opt.dataset.cat = e.cat;
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

cargarEjercicios('todos'); // Inicializar

$('btn-add-ej').addEventListener('click', () => {
    const selEj = $('sel-ejercicio');
    const series = $('ej-series').value.trim();
    const reps = $('ej-repeticiones').value.trim();

    if (!selEj.value) { mostrarAlerta('Seleccione un ejercicio.'); return; }
    if (!series || !soloNumeros(series) || parseInt(series) < 1) { mostrarAlerta('Ingrese un número válido de series (mínimo 1).'); return; }
    if (!reps || !soloNumeros(reps) || parseInt(reps) < 1) { mostrarAlerta('Ingrese un número válido de repeticiones (mínimo 1).'); return; }

    const ejercicio = EJERCICIOS_DB.find(e => e.id == selEj.value);
    const catLabel = catActiva !== 'todos' ? catActiva
        : ejercicio?.cat || '—';

    const tbody = $('ej-tbody');
    const emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    const tr = document.createElement('tr');
    tr.innerHTML = `
    <td>${ejercicio?.nombre || '—'}</td>
    <td style="text-transform:capitalize">${catLabel}</td>
    <td>${parseInt(series)}</td>
    <td>${parseInt(reps)}</td>
    <td>
      <button class="btn-tbl btn-tbl-del" title="Quitar ejercicio"
        onclick="quitarFilaEj(this)">🗑️</button>
    </td>
  `;
    tbody.appendChild(tr);

    selEj.value = '';
    $('ej-series').value = '';
    $('ej-repeticiones').value = '';
    mostrarAlerta('Ejercicio asignado.', 'ok');
});

function quitarFilaEj(btn) {
    const tr = btn.closest('tr');
    tr.remove();
    if (!$('ej-tbody').querySelector('tr')) {
        $('ej-tbody').innerHTML = '<tr class="empty-row"><td colspan="5">No hay ejercicios asignados.</td></tr>';
    }
}

/* ═══════════════════════════════════════════
   TAB 3 — SELECCIÓN DE MÚSCULO (SVG interactivo)
════════════════════════════════════════════ */
let zonaSeleccionada = null;
let generoActual = 'masculino';
let vistaActual = 'anterior';

/* Definición de zonas musculares con colores y nombres */
const ZONAS = {
    anterior: [
        { id: 'pectoral', nombre: 'Pectoral Mayor', color: '#6ee7b7', path: 'M 100 110 Q 115 100 130 110 Q 130 135 115 140 Q 100 135 100 110 Z M 160 110 Q 175 100 190 110 Q 190 135 175 140 Q 160 135 160 110 Z' },
        { id: 'deltoides-ant', nombre: 'Deltoides Anterior', color: '#fbbf24', path: 'M 90 105 Q 95 90 110 95 Q 110 115 100 120 Q 90 115 90 105 Z M 180 105 Q 185 90 200 95 Q 200 115 190 120 Q 180 115 180 105 Z' },
        { id: 'biceps', nombre: 'Bíceps Braquial', color: '#f87171', path: 'M 78 130 Q 82 120 92 125 Q 95 155 88 160 Q 78 155 78 130 Z M 198 130 Q 202 120 212 125 Q 215 155 208 160 Q 198 155 198 130 Z' },
        { id: 'abdominales', nombre: 'Abdominales', color: '#60a5fa', path: 'M 110 145 Q 145 140 180 145 L 178 200 Q 145 205 112 200 Z' },
        { id: 'cuadriceps', nombre: 'Cuádriceps', color: '#a78bfa', path: 'M 105 240 Q 120 235 135 240 L 138 310 Q 122 315 108 310 Z M 155 240 Q 170 235 185 240 L 188 310 Q 172 315 158 310 Z' },
        { id: 'tibial', nombre: 'Tibial Anterior', color: '#34d399', path: 'M 108 318 Q 118 314 128 318 L 126 370 Q 116 374 106 370 Z M 162 318 Q 172 314 182 318 L 180 370 Q 170 374 160 370 Z' },
    ],
    posterior: [
        { id: 'trapecio', nombre: 'Trapecio', color: '#fbbf24', path: 'M 105 95 Q 145 85 185 95 Q 175 120 145 125 Q 115 120 105 95 Z' },
        { id: 'dorsales', nombre: 'Dorsal Ancho', color: '#f87171', path: 'M 100 130 Q 115 125 135 135 L 130 190 Q 112 195 95 185 Z M 190 130 Q 175 125 155 135 L 160 190 Q 178 195 195 185 Z' },
        { id: 'gluteos', nombre: 'Glúteos', color: '#a78bfa', path: 'M 108 205 Q 145 198 182 205 L 180 250 Q 145 256 110 250 Z' },
        { id: 'isquiotibial', nombre: 'Isquiotibiales', color: '#60a5fa', path: 'M 108 258 Q 123 253 138 258 L 136 318 Q 120 323 106 318 Z M 152 258 Q 167 253 182 258 L 180 318 Q 164 323 150 318 Z' },
        { id: 'gemelos', nombre: 'Gemelos', color: '#6ee7b7', path: 'M 108 325 Q 120 320 130 325 L 128 375 Q 118 380 106 375 Z M 160 325 Q 172 320 182 325 L 180 375 Q 170 380 158 375 Z' },
        { id: 'triceps', nombre: 'Tríceps Braquial', color: '#34d399', path: 'M 78 130 Q 83 120 92 126 Q 93 158 85 163 Q 77 158 78 130 Z M 198 130 Q 203 120 212 126 Q 213 158 205 163 Q 197 158 198 130 Z' },
    ]
};

/* Cuerpo humano SVG base (silueta simple) */
function generarSVG(genero, vista) {
    const zonas = ZONAS[vista];

    // Silueta diferente por género (sutil)
    const cabezaRx = genero === 'femenino' ? 20 : 18;
    const cuerpoPath = genero === 'femenino'
        ? 'M 115 165 Q 100 175 98 220 Q 96 240 108 245 L 108 390 L 130 390 L 130 295 L 145 295 L 145 390 L 167 390 L 167 245 Q 179 240 182 220 Q 180 175 175 165 Z'
        : 'M 112 165 Q 95 175 93 215 Q 91 240 108 245 L 108 390 L 130 390 L 130 295 L 145 295 L 145 390 L 167 390 L 167 245 Q 199 240 197 215 Q 195 175 178 165 Z';

    const brazosPath = 'M 95 165 Q 82 168 76 210 Q 74 230 78 240 L 92 240 L 94 185 Q 108 170 115 165 Z M 195 165 Q 208 168 214 210 Q 216 230 212 240 L 198 240 L 196 185 Q 182 170 175 165 Z';

    let zonasSVG = zonas.map(z =>
        `<path class="zona-muscular" data-zona="${z.id}" data-nombre="${z.nombre}"
       d="${z.path}" fill="${z.color}" stroke="rgba(0,0,0,0.15)" stroke-width="0.8"
       role="button" tabindex="0" aria-label="${z.nombre}"/>`
    ).join('\n');

    return `
<svg viewBox="60 60 170 380" xmlns="http://www.w3.org/2000/svg" aria-label="Cuerpo humano ${genero} vista ${vista}">
  <!-- Cabeza -->
  <ellipse cx="145" cy="82" rx="${cabezaRx}" ry="22" fill="#f5cba7" stroke="#c8a882" stroke-width="1"/>
  <!-- Cuello -->
  <rect x="137" y="102" width="16" height="14" rx="4" fill="#f5cba7"/>
  <!-- Torso -->
  <path d="${cuerpoPath}" fill="#e8d5c4" stroke="#c8a882" stroke-width="1"/>
  <!-- Brazos -->
  <path d="${brazosPath}" fill="#e8d5c4" stroke="#c8a882" stroke-width="1"/>
  <!-- Zonas musculares interactivas -->
  ${zonasSVG}
  <!-- Manos -->
  <ellipse cx="75"  cy="248" rx="8" ry="11" fill="#f5cba7" stroke="#c8a882" stroke-width="1"/>
  <ellipse cx="215" cy="248" rx="8" ry="11" fill="#f5cba7" stroke="#c8a882" stroke-width="1"/>
  <!-- Pies -->
  <ellipse cx="120" cy="394" rx="14" ry="6" fill="#f5cba7" stroke="#c8a882" stroke-width="1"/>
  <ellipse cx="160" cy="394" rx="14" ry="6" fill="#f5cba7" stroke="#c8a882" stroke-width="1"/>
</svg>`;
}

function renderBody() {
    const canvas = $('body-canvas');
    canvas.innerHTML = generarSVG(generoActual, vistaActual);

    // Eventos en zonas
    canvas.querySelectorAll('.zona-muscular').forEach(zona => {
        zona.addEventListener('click', () => seleccionarZona(zona));
        zona.addEventListener('keydown', e => { if (e.key === 'Enter' || e.key === ' ') seleccionarZona(zona); });
    });
}

function seleccionarZona(zona) {
    // Quitar selección previa
    $('body-canvas').querySelectorAll('.zona-muscular').forEach(z => z.classList.remove('selected'));

    zona.classList.add('selected');
    zonaSeleccionada = { id: zona.dataset.zona, nombre: zona.dataset.nombre };

    $('zona-sel-display').value = zonaSeleccionada.nombre;
    $('zona-nombre-texto').textContent = zonaSeleccionada.nombre;
}

/* Selector género */
document.querySelectorAll('.genero-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.genero-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        generoActual = btn.dataset.genero;
        zonaSeleccionada = null;
        $('zona-sel-display').value = '';
        $('zona-nombre-texto').textContent = 'Ninguna zona seleccionada';
        renderBody();
    });
});

/* Selector vista */
document.querySelectorAll('.vista-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('.vista-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        vistaActual = btn.dataset.vista;
        zonaSeleccionada = null;
        $('zona-sel-display').value = '';
        $('zona-nombre-texto').textContent = 'Ninguna zona seleccionada';
        renderBody();
    });
});

/* Agregar zona a tabla */
$('btn-add-musculo').addEventListener('click', () => {
    if (!zonaSeleccionada) { mostrarAlerta('Seleccione una zona en el cuerpo.'); return; }

    const tratSelOpts = Array.from($('tratamiento-select').selectedOptions);
    if (!tratSelOpts.length) { mostrarAlerta('Seleccione al menos un tratamiento.'); return; }

    const tratamientos = tratSelOpts.map(o => o.text).join(', ');
    const desc = $('musculo-desc').value.trim();

    const tbody = $('musculo-tbody');
    const emptyRow = tbody.querySelector('.empty-row');
    if (emptyRow) emptyRow.remove();

    const tr = document.createElement('tr');
    tr.innerHTML = `
    <td><strong>${zonaSeleccionada.nombre}</strong><br/><small style="color:var(--muted);text-transform:capitalize">${vistaActual}</small></td>
    <td style="font-size:.8rem">${tratamientos}</td>
    <td title="${desc}">${desc ? desc.slice(0, 40) + (desc.length > 40 ? '…' : '') : '—'}</td>
    <td>
      <button class="btn-tbl btn-tbl-del" title="Quitar zona" onclick="quitarFilaMusculo(this)">🗑️</button>
    </td>
  `;
    tbody.appendChild(tr);

    // Limpiar
    $('zona-sel-display').value = '';
    $('zona-nombre-texto').textContent = 'Ninguna zona seleccionada';
    $('musculo-desc').value = '';
    $('tratamiento-select').selectedIndex = -1;
    $('body-canvas').querySelectorAll('.zona-muscular').forEach(z => z.classList.remove('selected'));
    zonaSeleccionada = null;

    mostrarAlerta('Zona muscular registrada.', 'ok');
});

function quitarFilaMusculo(btn) {
    const tr = btn.closest('tr');
    tr.remove();
    if (!$('musculo-tbody').querySelector('tr')) {
        $('musculo-tbody').innerHTML = '<tr class="empty-row"><td colspan="4">Sin zonas registradas. Seleccione un músculo en el cuerpo.</td></tr>';
    }
}

/* ── Init ── */
renderBody();

/* Exponer funciones globales referenciadas en HTML inline */
window.abrirEditar = abrirEditar;
window.quitarFilaDiag = quitarFilaDiag;
window.quitarFilaEj = quitarFilaEj;
window.quitarFilaMusculo = quitarFilaMusculo;
window.pedirConfirmacionEliminar = pedirConfirmacionEliminar;