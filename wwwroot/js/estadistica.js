let chartVentas = null;
let chartGanancias = null;
let chartProductos = null;
let chartFormasDePago = null;

document.addEventListener("DOMContentLoaded", function () {
    cargarEstadisticas(100); // Valor por defecto: últimos 3 meses

    document.getElementById("btnContinuar").addEventListener("click", function () {
        const periodo = parseInt(document.getElementById("tiempoSelect").value);
        cargarEstadisticas(periodo);
    });
});

function cargarEstadisticas(tiempo) {
    fetch(`/EstadisticaApi/${negocioId}?tiempo=${tiempo}`)
        .then(res => res.json())
        .then(data => {

            // Destruir gráficos anteriores si existen
            if (chartVentas) chartVentas.destroy();
            if (chartGanancias) chartGanancias.destroy();
            if (chartProductos) chartProductos.destroy();
            if (chartFormasDePago) chartFormasDePago.destroy();

            // Crear gráficos nuevos
            chartVentas = crearGrafico(
                "graficoVentas",
                "bar",
                data.ventasPorPeriodo.map(x => x.periodo),
                data.ventasPorPeriodo.map(x => x.valor),
                "Ventas",
                "rgba(75, 192, 192, 0.7)"
            );

            chartGanancias = crearGrafico(
                "graficoGanacias",
                "bar",
                data.gananciasPorPeriodo.map(x => x.periodo),
                data.gananciasPorPeriodo.map(x => x.valor),
                "Ganancias",
                "rgba(76, 175, 80, 0.6)"
            );

            chartProductos = crearGrafico(
                "graficoProductos",
                "pie",
                data.productosMasVendidos.map(x => x.nombre),
                data.productosMasVendidos.map(x => x.cantidadVendida),
                "Productos más vendidos",
                ['#4CAF50', '#2196F3', '#FF9800', '#9C27B0', '#FF6F61 ']
            );

            chartFormasDePago = crearGrafico(
                "graficoFormasDePago",
                "pie",
                data.formasDePago.map(x => x.metodo),
                data.formasDePago.map(x => x.cantidad),
                "Formas de pago",
                ['#00796B', '#FBC02D', '#1976D2', '#D32F2F']
            );
        })
        .catch(error => console.error("Error al cargar estadísticas:", error));
}

function crearGrafico(canvasId, tipo, etiquetas, valores, etiquetaDataSet, colores) {
    const ctx = document.getElementById(canvasId).getContext("2d");

    return new Chart(ctx, {
        type: tipo,
        data: {
            labels: etiquetas,
            datasets: [{
                label: etiquetaDataSet,
                data: valores,
                backgroundColor: Array.isArray(colores) ? colores : [colores]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: tipo === 'pie' ? 'bottom' : 'top'
                }
            }
        }
    });
}