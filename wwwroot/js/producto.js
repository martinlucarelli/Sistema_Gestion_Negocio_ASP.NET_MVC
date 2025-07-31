

//#################################################### GET ####################################################
document.addEventListener("DOMContentLoaded", function () //El contentLoaded se encarga de ejecutar solamente cuando
{                                                         //el html esta cargado, todo lo que se coloque dentro se      
    cargarProductos();
                                                           //ejecutara automaticamente cuando cargue el html.
});

//Esta funcion se fija si el usuario es admin o no, se utiliza dentro de cargarProductos para que le oculte las columnas de modificar o elimiinar producto a los empleados
function OcultarColumnaAEmpleados() {
    if (!esAdminNegocio && !esInvitado) {
        const columnasAcciones = document.querySelectorAll("td:nth-child(5), th:nth-child(5)");
        columnasAcciones.forEach(celda => {
            celda.style.display = "none";
        });
    }
}

let productos = []; 
function cargarProductos() {
    fetch(`/ProductoApi/${negocioId}`)
        .then(res => res.json())
        .then(data => {

            productos = data;
            filtrarYOrdenar();


        })
        .catch(err => {
            console.error("Error al traer productos", err);
        });
}

function filtrarYOrdenar() {

    const texto = document.getElementById("inputBuscar").value.toLowerCase();
    const criterio = document.getElementById("selectOrdenar").value;

    let filtrados = productos.filter(p => p.nombre.toLowerCase().startsWith(texto));

    filtrados.sort((a, b) => {
        if (criterio === "nombre") return a.nombre.localeCompare(b.nombre);
        if (criterio === "precio") return a.precio - b.precio;
        if (criterio === "stock") return a.stock - b.stock;
        return 0;
    });

    mostrarProductos(filtrados);
}

function mostrarProductos(lista) {

    const tbody = document.querySelector("tbody");
    tbody.innerHTML = "";

    lista.forEach((item, index) => {
        const row = document.createElement("tr");
        const nombreSanitizado = item.nombre.replace(/'/g, "\\'");
        row.innerHTML = `
                    <td>${index + 1}</td>
                    <td>${item.nombre}</td>
                    <td>${item.precio}</td>
                    <td>${item.stock}</td>
                    <td>
                         <button class="btn btn-sm btn-primary me-2" onclick="modificarProducto('${item.idProducto}')"><i class="bi bi-pencil-square"></i> Modificar</button>
                         <button class="btn btn-sm btn-danger" onclick="eliminarProducto('${item.idProducto}', '${nombreSanitizado}')"><i class="bi bi-trash"></i> Eliminar</button>
                    </td>
                `;

        tbody.appendChild(row);
    });

    OcultarColumnaAEmpleados();
}

document.getElementById("inputBuscar").addEventListener("input", filtrarYOrdenar);
document.getElementById("selectOrdenar").addEventListener("change", filtrarYOrdenar);

//#################################################### POST ####################################################
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formAgregarProducto");

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const producto = {
            nombre: document.getElementById("nombre").value,
            precio: parseFloat(document.getElementById("precio").value),
            stock: parseInt(document.getElementById("stock").value),
            negocioId: negocioId
        };

        console.log("Producto a enviar:", producto);
        console.log("JSON:", JSON.stringify(producto));

        fetch(`/ProductoApi`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(producto)
        })
            .then(res => {
                if (!res.ok) throw new Error("Error al agregar producto");
                
            })
            .then(data => {
                console.log("Producto agregado:", data);
                
                const modal = bootstrap.Modal.getInstance(document.getElementById("modalAgregarProducto"));
                modal.hide();

                document.getElementById("formAgregarProducto").reset();

                cargarProductos();

            })
            .catch(err => {
                console.error("Error al enviar producto", err);
            });
    });
});

//#################################################### DELETE ####################################################

//Funcion que se llama cuadno se hace click en eliminar (en la lista)
function eliminarProducto(id, nombre) {
    console.log("ID del producto a eliminar:", id);
    idProductoAEliminar = id;
    const mensaje = `¿Estás seguro de eliminar el producto <strong>${nombre}</strong>?`;
    document.getElementById("mensajeConfirmacion").innerHTML = mensaje;

    const modal = new bootstrap.Modal(document.getElementById("modalConfirmarEliminacion"));
    modal.show();
}

//Funcion que se llama cuando se confirma la eliminacion de un producto (en el modal)
document.getElementById("btnConfirmarEliminar").addEventListener("click", function () {
    if (idProductoAEliminar !== null) {
        fetch(`/ProductoApi/${idProductoAEliminar}`, {
            method: "DELETE"
        })
            .then(res => {
                if (!res.ok) throw new Error("Error al eliminar el producto");
                return res.text(); // o .json() si devolvés algo
            })
            .then(() => {
                console.log("Producto eliminado");

                const modal = bootstrap.Modal.getInstance(document.getElementById("modalConfirmarEliminacion"));
                modal.hide();

                cargarProductos(); // Recargar la tabla
            })
            .catch(err => {
                console.error("Error al eliminar", err);
            });
    }
});

//#################################################### PUT ####################################################

//Esta funcion carga los elementos en el modal.
function modificarProducto(id) {
    fetch(`/ProductoApi/Detalle/${id}`)
        .then(res => res.json())
        .then(producto => {
            document.getElementById("idModificar").value = producto.idProducto;
            document.getElementById("nombreModificar").value = producto.nombre;
            document.getElementById("precioModificar").value = producto.precio;
            document.getElementById("stockModificar").value = producto.stock;

            const modal = new bootstrap.Modal(document.getElementById("modalModificarProducto"));
            modal.show();
        })
        .catch(err => {
            console.error("Error al cargar producto para modificar", err);
        });
}


document.getElementById("formModificarProducto").addEventListener("submit", function (e) {
    e.preventDefault();

    const id = document.getElementById("idModificar").value;

    const productoActualizado = {
        nombre: document.getElementById("nombreModificar").value,
        precio: parseFloat(document.getElementById("precioModificar").value),
        stock: parseInt(document.getElementById("stockModificar").value)
    };

    console.log("Enviando ID:", id);
    console.log("Producto actualizado:", productoActualizado);


    fetch(`/ProductoApi/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(productoActualizado)
    })
        .then(res => {
            if (!res.ok) throw new Error("Error al actualizar el producto");
            return res.text();
        })
        .then(() => {
            console.log("Producto actualizado");
            const modal = bootstrap.Modal.getInstance(document.getElementById("modalModificarProducto"));
            modal.hide();
            cargarProductos();
        })
        .catch(err => {
            console.error("Error al actualizar", err);
        });
});


