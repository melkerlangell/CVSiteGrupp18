//javascript för att lägga till och ta bort element vid skapande och redigering av cv


//funktion för att lägga till en ny kompetens
const addKompetens = () => {
    //hämtar avsnittet för kompetenser
    const container = document.getElementById('kompetenser-container');

    //index för antal kompetenser
    const index = container.children.length;

    //skapar nytt element, samma som för view 
    const newField = `
            <div class="input-group mb-2">
                <input name="Kompetenser[${index}]" class="form-control" />
                <button type="button" class="btn btn-danger" onclick="removeField(this)">Ta bort</button>
            </div>`;
    //injicerar det nya elementet
    container.insertAdjacentHTML('beforeend', newField);
}


//funktion för att lägga till ny utbildning
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

//funktion för att lägga till arbetserfarenhet
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

//funktion för att ta bort ett element
const removeField = (btn) => {
    const field = btn.parentElement;
    field.remove();
    updateIndexes();
}

//uppdatera indexeringen mot databasen, används efter borttagning av fält för att det ska bli rätt indexering
const updateIndexes = () => {
    const kompetenser = document.querySelectorAll('#kompetenser-container .input-group');
    kompetenser.forEach((field, index) => {
        field.querySelector('input').name = `Kompetenser[${index}]`;
    });

    const utbildningar = document.querySelectorAll('#utbildningar-container .border');
    utbildningar.forEach((field, index) => {
        field.querySelector('input[name*="Skola"]').name = `Utbildningar[${index}].Skola`;
        field.querySelector('input[name*="Titel"]').name = `Utbildningar[${index}].Titel`;
        field.querySelector('input[name*="Startdatum"]').name = `Utbildningar[${index}].Startdatum`;
        field.querySelector('input[name*="Slutdatum"]').name = `Utbildningar[${index}].Slutdatum`;
    });

    const erfarenheter = document.querySelectorAll('#erfarenheter-container .border');
    erfarenheter.forEach((field, index) => {
        field.querySelector('input[name*="Företag"]').name = `Erfarenheter[${index}].Företag`;
        field.querySelector('input[name*="Roll"]').name = `Erfarenheter[${index}].Roll`;
        field.querySelector('textarea[name*="Beskrivning"]').name = `Erfarenheter[${index}].Beskrivning`;
        field.querySelector('input[name*="Startdatum"]').name = `Erfarenheter[${index}].Startdatum`;
        field.querySelector('input[name*="Slutdatum"]').name = `Erfarenheter[${index}].Slutdatum`;
    });
}
