
document.addEventListener("DOMContentLoaded", function () {
    const hoy = new Date();
    const fechaHoy = hoy.toISOString().split('T')[0];

    document.getElementById("desde").value = fechaHoy;
    document.getElementById("hasta").value = fechaHoy;

    cargarVentas(fechaHoy, fechaHoy);

    document.getElementById("btnFiltrar").addEventListener("click", () => {
        const desde = document.getElementById("desde").value;
        const hasta = document.getElementById("hasta").value;

        if (desde && hasta) {
            cargarVentas(desde, hasta);
        }
    });
});

function cargarVentas(desde, hasta) {
    fetch(`/VentaApi/${negocioId}?desde=${desde}&hasta=${hasta}`)
        .then(res => {
            if (!res.ok) throw new Error("Error al cargar ventas");
            return res.json();
        })
        .then(data => {
            const tbody = document.querySelector("table tbody");
            tbody.innerHTML = "";

            if (data.ventas.length === 0) {
                tbody.innerHTML = `<tr><td colspan="5" class="text-center">No se encontraron ventas</td></tr>`;
                return;
            }

            data.ventas.forEach((venta, index) => {
                const fecha = venta.fecha ? new Date(venta.fecha).toLocaleString() : "Fecha desconocida";
                const total = typeof venta.total === "number" ? `$${venta.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }` : "$0.00";
                const formaPago = venta.nombreFormaPago || "Sin forma de pago";
                const nombreUsuario = venta.nombreUsuario || "Usuario desconocido";

                const fila = `
                    <tr>
                        <th scope="row">${index + 1}</th>
                        <td>${fecha}</td>
                        <td>${total}</td>
                        <td>${formaPago}</td>
                        <td>${nombreUsuario}</td>
                        <td> 
                              <a href="/VentaApi/factura/${venta.idVenta}" target="_blank" class="btn btn-sm btn-secondary">
                                 Ver factura
                             </a>
                        
                        </td>
                    </tr>
                `;
                tbody.innerHTML += fila;
            });

            // Mostrar resumen
            const resumenDiv = document.getElementById("resumenVentas");
            resumenDiv.innerHTML = "<h5>Resumen</h5>";

            data.resumenPorFormaPago.forEach(r => {
                resumenDiv.innerHTML += `<p><strong>${r.formaPago}:</strong> $${r.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;
            });

            resumenDiv.innerHTML += `<p><strong>Total general:</strong> $${data.totalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;
        })
        .catch(err => {
            console.error("Error al obtener ventas:", err);
        });
}
