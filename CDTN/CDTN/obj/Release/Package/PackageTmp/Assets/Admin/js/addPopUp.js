window.addEventListener("DOMContentLoaded", function () {
    const overlay = document.getElementById('overlay');
    const popUp = document.getElementById('popUp');
    const deletePopup = document.getElementById('deletePopUp');
    const changePopUp = document.getElementById("changePopUp");

    const addManager = document.getElementById('addPopup');
    const cancelPopup = document.querySelectorAll('.cancelPopup');
    const deleteBtns = document.querySelectorAll('.delete');
    const changeBtns = document.querySelectorAll('.change');

    function showPopup(popupElement) {
        if (!popupElement) {
            console.warn("Không tìm thấy phần tử popup");
            return;
        }
        if (overlay) overlay.style.display = 'block';
        popupElement.style.display = 'block';
        document.body.classList.add('overflow-hidden');
    }

    function hideAllPopups() {
        if (overlay) overlay.style.display = 'none';
        if (popUp) popUp.style.display = 'none';
        if (deletePopup) deletePopup.style.display = 'none';
        if (changePopUp) changePopUp.style.display = 'none';
        document.body.classList.remove('overflow-hidden');
    }

    if (addManager) {
        addManager.addEventListener('click', () => showPopup(popUp));
    }

    cancelPopup.forEach(btn => {
        btn.addEventListener('click', hideAllPopups);
    });

    changeBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const idMatch = this.getAttribute('onclick')?.match(/\d+/);
            if (idMatch && idMatch[0]) {
                const id = parseInt(idMatch[0]);
                loadData(id, function () {
                    showPopup(changePopUp);
                });
            } else {
                console.warn("Không tìm thấy ID để sửa.");
            }
        });
    });


    deleteBtns.forEach(btn => {
        btn.addEventListener('click', () => showPopup(deletePopup));
    });
});
