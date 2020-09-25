// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function AddToCart(name, equipmentType, rentalDays){
    let table = document.getElementById("cart_table");
    let rows = table.rows.length;
    let row = table.insertRow(rows);
    let cell1 = row.insertCell(0);
    let cell2 = row.insertCell(1);
    let cell3 = row.insertCell(2);
    cell1.innerHTML = name.innerText;
    cell2.innerHTML = equipmentType.innerText;
    cell2.ApiValue = equipmentType.getAttribute("ApiValue");
    cell3.innerHTML = rentalDays.firstChild.value;
}

function GenerateInvoice(){
    let xhttp = new XMLHttpRequest();
    let equipment = {
        equipment: GetCartEquipment() 
    };
    let message = JSON.stringify(equipment);
    xhttp.open("POST", "Home/GenerateInvoice", true);
    xhttp.setRequestHeader("Content-Type", "application/json");
    xhttp.send(message);

    xhttp.onload = function () {
        let result = xhttp.response;
        console.info("received: " + result);
        console.info(xhttp);
        download("Invoice.json", result);
    };
}

function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function GetCartEquipment(){
    let table = document.getElementById("cart_table");
    let equipmentItems = [];
    for (i = 1; i < table.rows.length; i++){
        let cells = table.rows[i].cells;
        equipmentItems[i-1] = {
            Name: cells[0].innerText,
            EquipmentType: parseInt(cells[1].ApiValue),
            RentalDays: parseInt(cells[2].innerText)
        };
    }
    return equipmentItems;
}
