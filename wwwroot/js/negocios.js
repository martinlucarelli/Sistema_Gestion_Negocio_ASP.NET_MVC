

function verUsuarioNegocio(negocioId) {

    fetch(`/NegocioApi/${negocioId}`)
        .then(res => res.json())
        .then(data => {
            const modalBody = document.querySelector("#modalUsuariosDeNegocio .modal-body");

            if (!data || data.length === 0) {
                modalBody.innerHTML = `<p class="text-muted">Este negocio no tiene usuarios vinculados.</p>`;
                return;
            }

            // Generar lista
            let listaHTML = `<ul class="list-group">`;
            data.forEach(usuario => {
                let rolTexto = "";
                switch (usuario.rol) {
                    case 1: rolTexto = "Admin general"; break;
                    case 2: rolTexto = "Admin negocio"; break;
                    case 3: rolTexto = "Empleado"; break;
                    default: rolTexto = "Desconocido";
                }

                listaHTML += `
                    <li class="list-group-item d-flex justify-content-between align-items-start">
                        <div>
                            <div><strong>${usuario.nombre}</strong></div>
                            <small class="text-muted">${usuario.correo}</small>
                        </div>
                        <span class="badge rounded-pill rolUsuario">${rolTexto}</span>
                    </li>`;
            });
            listaHTML += `</ul>`;

            modalBody.innerHTML = listaHTML;
        })
        .catch(err => {
            console.error("Error al cargar usuarios:", err);
            document.querySelector("#modalUsuariosDeNegocio .modal-body").innerHTML = `<p class="text-danger">Hubo un error al cargar los usuarios.</p>`;
        });

    const modal = new bootstrap.Modal(document.getElementById("modalUsuariosDeNegocio"));
    modal.show();
}

document.addEventListener("DOMContentLoaded", function () {
    const botonesUsuarios = document.querySelectorAll(".btn-usuarios-vinculados");

    botonesUsuarios.forEach(boton => {
        boton.addEventListener("click", function (e) {
            e.preventDefault();
            const negocioId = this.getAttribute("data-id");
            verUsuarioNegocio(negocioId);
        });
    });
});



///////////////////// #DELETE /////////////////////////

let idNegocioAEliminar = null;


document.querySelectorAll(".btn-abrir-modal-eliminar").forEach(boton => {
    boton.addEventListener("click", function () {
        idNegocioAEliminar = this.getAttribute("data-id");
        const nombreNegocio = this.getAttribute("data-nombre");

        document.getElementById("mensajeConfirmacion").textContent =
            `¿Estás seguro de que querés eliminar el negocio "${nombreNegocio}"?`;

        const modal = new bootstrap.Modal(document.getElementById("modalConfirmarEliminacion"));
        modal.show();
    });
});

document.getElementById("btnConfirmarEliminar").addEventListener("click", function () {
    if (!idNegocioAEliminar) return;

    fetch(`/NegocioApi/${idNegocioAEliminar}`, {
        method: "DELETE"
    })
        .then(res => {
            if (!res.ok) throw new Error("Error al eliminar el negocio");
            location.reload();
        })
        .catch(err => {
            console.error("Error:", err);
            alert("No se pudo eliminar el negocio.");
        });
});
