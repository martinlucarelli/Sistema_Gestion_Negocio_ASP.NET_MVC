let productosDisponibles = []; // Se llena con fetch
let detalleVenta = [];
let productoSeleccionado = null;

document.addEventListener("DOMContentLoaded", function () {
    fetch("/ProductoApi/" + negocioId)
        .then(res => res.json())
        .then(data => {
            productosDisponibles = data;
        });

    document.getElementById("busquedaProducto").addEventListener("input", function () {
        const busqueda = this.value.toLowerCase();
        const contenedorResultados = document.getElementById("resultadosBusqueda");
        contenedorResultados.innerHTML = ""; // Limpiar resultados

        if (busqueda.length === 0) return;

        const productosFiltrados = productosDisponibles.filter(p =>
            p.nombre.toLowerCase().includes(busqueda)
        );

        productosFiltrados.forEach(p => {
            const div = document.createElement("div");
            div.className = "border rounded p-2 mb-1 producto-item";
            div.style.cursor = "pointer";
            div.textContent = `${p.nombre} - $${p.precio}`;
            div.addEventListener("click", function () {
                productoSeleccionado = p;
                document.getElementById("btnAgregarDetalle").disabled = false;
                document.getElementById("busquedaProducto").value = `${p.nombre}`;
                contenedorResultados.innerHTML = ""; // Limpiar resultados al seleccionar
            });
            contenedorResultados.appendChild(div);
        });
    });

    document.getElementById("btnAgregarDetalle").addEventListener("click", function () {
        const cantidad = parseInt(document.getElementById("cantidadInput").value);

        if (!productoSeleccionado || cantidad < 1) return;

        const subtotal = productoSeleccionado.precio * cantidad;

        detalleVenta.push({
            productoId: productoSeleccionado.idProducto,
            nombre: productoSeleccionado.nombre,
            precio: productoSeleccionado.precio,
            cantidad: cantidad,
            subtotal: subtotal
        });

        renderDetalle();

        // Resetear
        document.getElementById("busquedaProducto").value = "";
        document.getElementById("cantidadInput").value = 1;
        document.getElementById("btnAgregarDetalle").disabled = true;
        productoSeleccionado = null;
    });

    document.getElementById("btnConfirmarVenta").addEventListener("click", function () {
        confirmarVenta(); 
    });
});

function renderDetalle() {
    const tbody = document.querySelector("#tablaDetalleVenta tbody");
    tbody.innerHTML = "";
    let total = 0;

    detalleVenta.forEach((item, index) => {
        total += item.subtotal;
        tbody.innerHTML += `
            <tr>
                <td>${item.nombre}</td>
                <td>$${item.precio}</td>
                <td>${item.cantidad}</td>
                <td>$${item.subtotal.toFixed(2)}</td>
                <td><button class="btn btn-sm btn-danger" onclick="eliminarDetalle(${index})">Eliminar</button></td>
            </tr>
        `;
    });

    document.getElementById("totalVenta").textContent = total.toFixed(2);
    document.getElementById("btnConfirmarVenta").disabled = detalleVenta.length === 0;

}

function eliminarDetalle(index) {
    detalleVenta.splice(index, 1);
    renderDetalle();
}




//################################## POST ################################################


function confirmarVenta() {
    const formaPagoId = document.getElementById("formaPagoSelect").value;
    if (!formaPagoId) {
        alert("Seleccione una forma de pago.");
        return;
    }

    if (detalleVenta.length === 0) {

        alert("Debe agregar al menos un producto a la venta.")
        return;
    }

    const venta = {
        usuarioId: usuarioId,
        negocioId: negocioId,
        formaPagoId: formaPagoId,
        total: detalleVenta.reduce((sum, item) => sum + item.subtotal, 0),
        detalle: detalleVenta.map(item => ({
            productoId: item.productoId,
            cantidadProductos: item.cantidad,
            subtotal: item.subtotal
        }))
    };

    fetch("/VentaApi", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(venta)
    }).then(res => {
        if (res.ok) {
            alert("Venta registrada correctamente");
            location.reload();
        } else {
            alert("Ocurrió un error al registrar la venta.");
        }
    });
}

