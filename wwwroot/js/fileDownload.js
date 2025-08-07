function downloadTextFile(content, filename) {
    const element = document.createElement('a');
    const file = new Blob([content], { type: 'text/plain' });
    element.href = URL.createObjectURL(file);
    element.download = filename;
    document.body.appendChild(element); // Append the element to trigger the download
    element.click();
    document.body.removeChild(element); // Remove the element after download
}

window.downloadBinaryFile = function (base64Data, filename) {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/octet-stream' });

    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = filename;
    a.click();
    URL.revokeObjectURL(a.href);
};
window.blazorFileDrop = {
    initDropZone: function (elementId, dotNetHelper) {
        const dropZone = document.getElementById(elementId);

        dropZone.addEventListener("dragover", function (e) {
            e.preventDefault();
            dropZone.classList.add("dragging");
        });

        dropZone.addEventListener("dragleave", function () {
            dropZone.classList.remove("dragging");
        });

        dropZone.addEventListener("drop", function (e) {
            e.preventDefault();
            dropZone.classList.remove("dragging");

            if (e.dataTransfer.files.length > 0) {
                const file = e.dataTransfer.files[0];
                const reader = new FileReader();
                reader.onload = function () {
                    dotNetHelper.invokeMethodAsync("LoadFromDragDrop", reader.result);
                };
                reader.readAsText(file);
            }
        });
    }
};