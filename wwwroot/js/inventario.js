const CLAVE = "ranibal_inventario_v1";

const materialesIniciales = [
    {
        codigo: "MAT-001",
        nombre: "Lámina acrílica",
        existencias: 25,
        unidad: "Unidad"
    },
    {
        codigo: "MAT-002",
        nombre: "Vinil adhesivo",
        existencias: 12,
        unidad: "Rollo"
    },
    {
        codigo: "MAT-003",
        nombre: "Perfil metálico",
        existencias: 40,
        unidad: "Unidad"
    }
];

const tabla = document.getElementById("tablaMateriales");
const buscador = document.getElementById("buscador");
const modal = document.getElementById("modalMaterial");
const formulario = document.getElementById("formMaterial");

let materiales = cargarMateriales();
let codigoEditando = null;

function cargarMateriales() {
    try {
        const guardados = localStorage.getItem(CLAVE);

        if (guardados === null) {
            localStorage.setItem(
                CLAVE,
                JSON.stringify(materialesIniciales)
            );

            return [...materialesIniciales];
        }

        const datos = JSON.parse(guardados);

        if (!Array.isArray(datos)) {
            throw new Error("Formato de inventario inválido");
        }

        return datos;
    } catch (error) {
        alert("No se pudo leer el inventario guardado.");
        console.error(error);
        return [];
    }
}


function guardarMateriales() {
    try {
        localStorage.setItem(CLAVE, JSON.stringify(materiales));
        return true;
    } catch (error) {
        alert("No se pudieron guardar los cambios en el navegador.");
        console.error(error);
        return false;
    }
}

function mostrarMateriales() {
    const busqueda = buscador.value.trim().toLowerCase();

    const filtrados = materiales.filter(material =>
        [
            material.codigo,
            material.nombre,
            material.unidad
        ].some(valor => valor.toLowerCase().includes(busqueda))
    );

    tabla.replaceChildren();

    filtrados.forEach(material => {
        const fila = document.createElement("tr");

        [
            material.codigo,
            material.nombre,
            material.existencias,
            material.unidad
        ].forEach(valor => {
            const celda = document.createElement("td");
            celda.textContent = valor;
            fila.appendChild(celda);
        });

        const acciones = document.createElement("td");

        const editar = document.createElement("button");
        editar.type = "button";
        editar.className = "btn-edit";
        editar.textContent = "Editar";
        editar.dataset.accion = "editar";
        editar.dataset.codigo = material.codigo;

        const eliminar = document.createElement("button");
        eliminar.type = "button";
        eliminar.className = "btn-edit";
        eliminar.textContent = "Eliminar";
        eliminar.dataset.accion = "eliminar";
        eliminar.dataset.codigo = material.codigo;

        acciones.append(editar, eliminar);
        fila.appendChild(acciones);
        tabla.appendChild(fila);
    });

    document.getElementById("sinResultados").hidden =
        filtrados.length !== 0;
}


function nuevoMaterial() {
    formulario.reset();
    codigoEditando = null;

    document.getElementById("tituloModal").textContent =
        "Nuevo material";

    document.getElementById("codigo").readOnly = false;
    modal.hidden = false;
    document.getElementById("codigo").focus();
}

function editarMaterial(codigo) {
    const material = materiales.find(m => m.codigo === codigo);
    if (!material) return;

    formulario.reset();
    codigoEditando = codigo;

    document.getElementById("tituloModal").textContent =
        "Editar material";

    document.getElementById("codigo").value = material.codigo;
    document.getElementById("codigo").readOnly = true;

    document.getElementById("nombre").value = material.nombre;
    document.getElementById("existencias").value =
        material.existencias;
    document.getElementById("unidad").value = material.unidad;

    modal.hidden = false;
    document.getElementById("nombre").focus();
}

function cerrarModal() {
    modal.hidden = true;
    codigoEditando = null;
    formulario.reset();
    document.getElementById("codigo").readOnly = false;
}

formulario.addEventListener("submit", function(event) {
    event.preventDefault();

    const codigo = document.getElementById("codigo")
        .value.trim().toUpperCase();

    const nombre = document.getElementById("nombre")
        .value.trim();

    const existenciasTexto =
        document.getElementById("existencias").value;

    const unidad = document.getElementById("unidad").value;

    if (!codigo || !nombre || existenciasTexto === "" || !unidad) {
        alert("Completa todos los campos.");
        return;
    }

    const existencias = Number(existenciasTexto);

    if (!Number.isSafeInteger(existencias) || existencias < 0) {
        alert("Las existencias deben ser un número entero igual o mayor que cero.");
        return;
    }

    const duplicado = materiales.some(m =>
        m.codigo.toUpperCase() === codigo &&
        m.codigo !== codigoEditando
    );

    if (duplicado) {
        alert("Ya existe un material con ese código.");
        return;
    }

    const anterior = materiales;

    const material = { codigo, nombre, existencias, unidad };

    if (codigoEditando !== null) {
        materiales = materiales.map(m =>
            m.codigo === codigoEditando ? material : m
        );
    } else {
        materiales = [...materiales, material];
    }

    if (!guardarMateriales()) {
        materiales = anterior;
        return;
    }

    mostrarMateriales();
    cerrarModal();
});

tabla.addEventListener("click", function(event) {
    const boton = event.target.closest("button[data-accion]");
    if (!boton) return;

    const codigo = boton.dataset.codigo;

    if (boton.dataset.accion === "editar") {
        editarMaterial(codigo);
    }

    if (boton.dataset.accion === "eliminar") {
        const confirmar = confirm(
            `¿Deseas eliminar el material ${codigo}?`
        );

        if (!confirmar) return;

        const anterior = materiales;

        materiales = materiales.filter(m => m.codigo !== codigo);

        if (!guardarMateriales()) {
            materiales = anterior;
            return;
        }

        mostrarMateriales();
    }
});

document.getElementById("btnNuevo")
    .addEventListener("click", nuevoMaterial);

document.getElementById("btnCerrar")
    .addEventListener("click", cerrarModal);

document.getElementById("btnCancelar")
    .addEventListener("click", cerrarModal);

buscador.addEventListener("input", mostrarMateriales);

modal.addEventListener("click", event => {
    if (event.target === modal) cerrarModal();
});

document.addEventListener("keydown", event => {
    if (event.key === "Escape" && !modal.hidden) {
        cerrarModal();
    }
});

mostrarMateriales();