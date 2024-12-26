const addKompetens = () => {
    const container = document.getElementById('kompetenser-container');
    const index = container.children.length;
    const newField = `
            <div class="input-group mb-2">
                <input name="Kompetenser[${index}]" class="form-control" />
                <button type="button" class="btn btn-danger" onclick="removeField(this)">Ta bort</button>
            </div>`;
    container.insertAdjacentHTML('beforeend', newField);
}

const addUtbildning = () => {
    const container = document.getElementById('utbildningar-container');
    const index = container.children.length;
    const newField = `
            <div class="border p-3 mb-3">
                <div class="mb-2">
                    <label class="form-label">Lärosäte:</label>
                    <input name="Utbildningar[${index}].Skola" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Titel:</label>
                    <input name="Utbildningar[${index}].Titel" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Startdatum:</label>
                    <input name="Utbildningar[${index}].Startdatum" type="date" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Slutdatum:</label>
                    <input name="Utbildningar[${index}].Slutdatum" type="date" class="form-control" />
                </div>
                <button type="button" class="btn btn-danger" onclick="removeField(this)">Ta bort</button>
            </div>`;
    container.insertAdjacentHTML('beforeend', newField);
}

const addErfarenhet = () => {
    const container = document.getElementById('erfarenheter-container');
    const index = container.children.length;
    const newField = `
            <div class="border p-3 mb-3">
                <div class="mb-2">
                    <label class="form-label">Företag:</label>
                    <input name="Erfarenheter[${index}].Företag" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Roll:</label>
                    <input name="Erfarenheter[${index}].Roll" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Beskrivning:</label>
                    <textarea name="Erfarenheter[${index}].Beskrivning" class="form-control"></textarea>
                </div>
                <div class="mb-2">
                    <label class="form-label">Startdatum:</label>
                    <input name="Erfarenheter[${index}].Startdatum" type="date" class="form-control" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Slutdatum:</label>
                    <input name="Erfarenheter[${index}].Slutdatum" type="date" class="form-control" />
                </div>
                <button type="button" class="btn btn-danger" onclick="removeField(this)">Ta bort</button>
            </div>`;
    container.insertAdjacentHTML('beforeend', newField);
}

const removeField = (btn) => {
    const field = btn.parentElement;
    field.remove();
}

const form = document.getElementById("cv-form");
form.addEventListener("submit", function (e) {
    const kompetenser = document.querySelectorAll("[name='Kompetenser[]']");
    const utbildningar = document.querySelectorAll("[name='Utbildningar[]']");
    const erfarenheter = document.querySelectorAll("[name='Erfarenheter[]']");

    if (kompetenser.length === 0 || utbildningar.length === 0 || erfarenheter.length === 0) {
        e.preventDefault();
        alert("Alla fält måste fyllas i.");
    }
});
