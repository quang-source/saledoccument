function sortTable(arrayData, columnKey, order) {
    return arrayData.sort((a, b) => {
        let valA = a[columnKey]; // Use the column key to access the value
        let valB = b[columnKey];

        // Handle case-insensitive string comparison
        if (typeof valA === 'string') {
            valA = valA.toLowerCase();
            valB = valB.toLowerCase();
        }

        // Perform the sorting based on the order
        if (order === 'asc') {
            return valA < valB ? -1 : valA > valB ? 1 : 0;
        } else {
            return valA < valB ? 1 : valA > valB ? -1 : 0;
        }
    });
}
function updateSortIndicator(columnIndex, order) {
    // Remove existing sort icons
    $('th .sort-indicator').removeClass('bi-sort-up bi-sort-down').addClass('bi');  // Reset all

    // Get the target column header
    const sortedColumn = $('th').eq(columnIndex);
    const indicator = sortedColumn.find('.sort-indicator');

    // Update the sort icon based on the order
    if (order === 'asc') {
        indicator.removeClass('bi-sort-down').addClass('bi-sort-up');  // Ascending icon
    } else {
        indicator.removeClass('bi-sort-up').addClass('bi-sort-down');  // Descending icon
    }
}
function handleHeaderClick() {
    $('th').click(function () {
        const columnIndex = $(this).data('column');  // Get the column index
        const columnKey = $(this).data('filter');  // Get the column index
        const tableData = getTableData();            // Get the data (this could be your array)
        if (currentSortColumn === columnIndex) {
            sortOrder = (sortOrder === 'asc') ? 'desc' : 'asc';  // Toggle sort order
        } else {
            sortOrder = 'asc';  // Default to ascending when a new column is clicked
        }
        const sortedData = sortTable(tableData, columnKey, sortOrder);
        drawTable(sortedData);
        currentSortColumn = columnIndex;
        updateSortIndicator(columnIndex, sortOrder);
    });
}
function handelFilter() {
    $("#myTable").on("change", "select.filter", function () {
        let columnIndex = $(this).data("column");
        let selectedValue = $(this).val();
        let filteredArray = arr;
        $("#myTable thead th select.filter").each(function (v, k) {
            var columnToFilter = $(k).data("filter");
            var valueToFilter = $(k).val();
            if (valueToFilter != 'ALL' && valueToFilter != '') {
                filteredArray = filterByKeyValue(filteredArray, columnToFilter, valueToFilter);
            }
        });
        drawTable(filteredArray);
        const oldval = $(this).val();
        populateFilters();
        $(this).val(oldval);
    });
}
function checkAndHighlightText(text, searchValue) {
    text = String(text);  // Ensures text is treated as a string
    if (text == 'null') {
        text = '';
    }
    if (!searchValue) return text;
    const escapedSearchValue = searchValue.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    const regex = new RegExp(`(${escapedSearchValue})`, 'gi');
    return text.replace(regex, '<span class="highlight">$1</span>');
}
function filterByKeyValue(array, key, value) {
    return array.filter(item => item[key].trim() === value.trim());
}
function drawTable(arrayData, searchValue = '') {
    const $tbody = $('#myTable tbody');
    $('#myTable tbody tr').remove();
    arrayData.forEach(row => {
        const $tr = $('<tr></tr>');
        $tr.append(`<td title="${row.customer_group}">${checkAndHighlightText(row.customer_group, searchValue)}</td>`);
        $tr.append(`<td title="${row.customer_code}">${checkAndHighlightText(row.customer_code, searchValue)}</td>`);
        $tr.append(`<td title="${row.order_type}">${checkAndHighlightText(row.order_type, searchValue)}</td>`);
        $tr.append(`<td title="${row.customer_order_number}">${checkAndHighlightText(row.customer_order_number, searchValue)}</td>`);
        $tr.append(`<td title="${row.order_id}">${checkAndHighlightText(row.order_id?.trim(), searchValue)}</td>`);
        $tr.append(`<td title="${row.lop}">${checkAndHighlightText(row.lop, searchValue)}</td>`);
        $tr.append(`<td><a data-filter="Packing" href="javascript:void(0)">${checkAndHighlightText(row.packing, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Non Moving" href="javascript:void(0)">${checkAndHighlightText(row.non_moving, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Repairing"  href="javascript:void(0)">${checkAndHighlightText(row.repairing, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Finishing"  href="javascript:void(0)">${checkAndHighlightText(row.finishing, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Pre Finishing"  href="javascript:void(0)">${checkAndHighlightText(row.pre_finishing, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Setting"  href="javascript:void(0)">${checkAndHighlightText(row.setting, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Missing Component"  href="javascript:void(0)">${checkAndHighlightText(row.missing_component, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Stones"  href="javascript:void(0)">${checkAndHighlightText(row.stones, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Post Preparation"  href="javascript:void(0)">${checkAndHighlightText(row.post_preparation, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Preparation"  href="javascript:void(0)">${checkAndHighlightText(row.preparation, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Casting"  href="javascript:void(0)">${checkAndHighlightText(row.casting, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Wax"  href="javascript:void(0)">${checkAndHighlightText(row.wax, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Wax Setting"  href="javascript:void(0)">${checkAndHighlightText(row.wax_setting, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="Planning"  href="javascript:void(0)">${checkAndHighlightText(row.planning, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="others"  href="javascript:void(0)">${checkAndHighlightText(row.others, searchValue)}</a></td>`);
        $tr.append(`<td><a data-filter="total"  href="javascript:void(0)">${checkAndHighlightText(row.total, searchValue)}</a></td>`);

        $tbody.append($tr);
    });
    mergeTableCells("#myTable tbody", 4);
}
function populateFilters() {
    $("#myTable thead th").each(function (index) {
        let select = $(this).find("select.filter");
        $($(this).find("select.filter option")).each(function (i, k) {
            $(k).remove();
        });
        let uniqueValues = new Set();

        $("#myTable tbody tr:visible").each(function () {
            let cellValue = $(this).find("td").eq(index).clone().find('span').remove().end().text();
            uniqueValues.add(cellValue);
        });
        select.append(' <option value="">All</option>');
        uniqueValues.forEach(function (value) {
            select.append('<option value="' + value + '">' + value + '</option>');
        });
    });
}
function mergeTableCells(selector, lastColumnIndex) {
    let rows = $(selector).find("tr");

    for (let columnIndex = 0; columnIndex <= lastColumnIndex; columnIndex++) {
        let firstCell = null;
        let rowspan = 1;

        rows.each(function (i, row) {
            let currentCell = $(row).find("td").eq(columnIndex);
            currentCell.data("col", columnIndex);

            let nameAttr = 'col';
            for (let i = 0; i <= columnIndex; i++) {
                let text = $(row).find("td").eq(i).clone().find('span').remove().end().text().trim();
                nameAttr += `_${text}`;
            }
            currentCell.attr("name", nameAttr);
            let isSameGroup = true;
            for (let prevColumnIndex = 0; prevColumnIndex < columnIndex; prevColumnIndex++) {
                let prevCell = $(row).find("td").eq(prevColumnIndex);
                let prevFirstCell = $(firstCell).closest("tr").find("td").eq(prevColumnIndex);
                if (prevCell.attr("name") !== prevFirstCell.attr("name")) {
                    isSameGroup = false;
                    break;
                }
            }
            if (firstCell && currentCell.text().trim() === firstCell.text().trim() && isSameGroup) {
                rowspan++;
                firstCell.attr("rowspan", rowspan);
                currentCell.hide();
            } else {
                if (firstCell && rowspan > 1) {
                    firstCell.html('<span class="toggle-icon">-</span>' + firstCell.html());
                }
                rowspan = 1;
                firstCell = currentCell;
            }

            firstCell.attr("data-origin-rowspan", rowspan);
        });
        if (firstCell && rowspan > 1) {
            firstCell.html('<span class="toggle-icon">-</span>' + firstCell.html());
        }
    }
}
function getTableData() {
    return arr;
}
function handelToggleIconClick() {
    $("#myTable").on("click", ".toggle-icon", function () {
        let icon = $(this);
        let cell = icon.closest("td");
        let currentRow = cell.closest("tr");
        const cellName = cell.attr("name");
        const cellColIdx = cell.data("col");
        if (icon.text() === "-") {
            icon.text("+");
            let rowspan = parseInt(cell.attr("rowspan"));
            let nextAll = currentRow.nextAll(":visible");
            for (let i = 1; i < rowspan; i++) {
                nextAll.eq(i - 1).hide();
            }
            cell.attr("rowspan", 1);
            let hiddenRowCount = rowspan - 1;

            // applied logic for all col after (0: CustGroup): reducing prev columns' rowspan
            if (cellColIdx > 0) {
                for (let i = cellColIdx; i > 0; i--) {
                    let prevCol = currentRow.find("td").eq(i - 1);

                    let td = $(`[name="${prevCol.attr("name")}"]:visible`).first();

                    let currentRowspan = parseInt(td.attr("rowspan"));
                    td.attr("rowspan", currentRowspan - hiddenRowCount);
                }
            }

            // applied logic for all col before (4: OrderID): hide related cells after current cell (only td under 0, 1, 2, 3, 4 column)
            if (cellColIdx < 4) {
                let colspan = 1;
                const children = $(`[name*="${cellName}"]:not([name="${cellName}"])`);

                children.each(function (i, child) {
                    $(child).hide();
                });

                cell.attr("colspan", 4 - cellColIdx + 1);
            }

            // sum all qty columns, 6 and 50 are hardcode
            for (let i = 6; i <= 50; i++) {
                let qty = parseInt(currentRow.find("td").eq(i).text()) || 0;

                let nextAll = currentRow.nextAll();
                for (let j = 1; j < rowspan; j++) {
                    let nextQty = parseInt(nextAll.eq(j - 1).find("td").eq(i).text()) || 0;
                    qty += nextQty;
                }

                if (qty > 0) {
                    currentRow.find("td").eq(i).html('<a href="javascript:void(0)">' + qty + '</a>');
                }
                else {
                    currentRow.find("td").eq(i).text('');
                }
            }

        } else {
            icon.text("-");
            let rowspan = parseInt(cell.attr("data-origin-rowspan"));
            let hiddenRowCount = rowspan - 1;

            // applied logic for all col before (4: OrderID): show related cells after current cell (only td under 0, 1, 2, 3, 4 column)
            if (cellColIdx <= 4) {
                const children = $(`td[name*="${cellName}"]:not([name="${cellName}"])[data-origin-rowspan]`);

                let ignoredNames = []

                children.each(function (i, child) {
                    let childIconText = $(child).find('span.toggle-icon').text();
                    if (childIconText === '+') {
                        ignoredNames.push($(child).attr("name"));

                        hiddenRowCount -= (parseInt($(child).attr("data-origin-rowspan")) - 1);
                    }
                });

                children.each(function (i, child) {
                    let childName = $(child).attr("name");

                    // Check if the childName does not contain any name in ignoredNames
                    let isIgnored = ignoredNames.some(ignoredName => childName.includes(ignoredName) && childName != ignoredName);

                    if (!isIgnored) {
                        $(child).show(); // Show the child if it's not ignored
                    }
                });

                cell.attr("rowspan", hiddenRowCount + 1);
                cell.attr("colspan", 1);
            }

            // Show the rows under the current merged cell
            let nextAll = currentRow.nextAll(":hidden");
            for (let i = 1; i < hiddenRowCount + 1; i++) {
                nextAll.eq(i - 1).show();
            }

            // applied logic for all col after (0: CustGroup): increasing prev columns' rowspan
            if (cellColIdx > 0) {
                for (let i = cellColIdx; i > 0; i--) {
                    let prevCol = currentRow.find("td").eq(i - 1);

                    let td = $(`[name="${prevCol.attr("name")}"]:visible`).first();

                    let currentRowspan = parseInt(td.attr("rowspan"));
                    td.attr("rowspan", currentRowspan + hiddenRowCount);
                }
            }

            // de-sum all qty columns  (6 -> 19)
            for (let i = 6; i <= 22; i++) {
                let qty = parseInt(currentRow.find("td").eq(i).text()) || 0;

                let nextAll = currentRow.nextAll(":visible");
                for (let j = 1; j < rowspan; j++) {
                    let prevQty = parseInt(nextAll.eq(j - 1).find("td").eq(i).text()) || 0;
                    qty -= prevQty;
                }

                if (qty > 0) {
                    currentRow.find("td").eq(i).html('<a href="javascript:void(0)">' + qty + '</a>');
                }
                else {
                    currentRow.find("td").eq(i).text('');
                }
            }
        }

    });
}
