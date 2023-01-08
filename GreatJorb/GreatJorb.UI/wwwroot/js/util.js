async function showModal(id)
{
    var modalElement = document.getElementById(id);
    var modal = new bootstrap.Modal(modalElement, {});
    modal.show();
   
    await waitForModalClose(modalElement);
}

function waitForModalClose(modalElement) {

    var closed = false;

    modalElement.addEventListener('hidden.bs.modal', function (event) {
        closed = true;
    })

    const poll = resolve => {
        if (closed) resolve();
        else setTimeout(_ => poll(resolve), 400);
    }

    return new Promise(poll);
}