$(document).ready(function () {
    var $form = $('.needs-validation');

    // HTML5 custom validation
    $form.on('submit', function (event) {
        if (!this.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }
        $(this).addClass('was-validated');
    });

    // Update totals and price warning
    function updateTotals() {
        var quantity = parseFloat($('#Quantity').val()) || 0,
            costPrice = parseFloat($('#CostPrice').val()) || 0,
            sellingPrice = parseFloat($('#SellingPrice').val()) || 0,
            totalCost = quantity * costPrice,
            totalSelling = quantity * sellingPrice;

        $('#totalCost').text(totalCost.toFixed(2));
        $('#totalSelling').text(totalSelling.toFixed(2));

        // Validate that the selling price is not less than the cost price.
        if (sellingPrice < costPrice) {
            $('#SellingPrice').addClass('is-invalid');
            $('#priceWarning').text('سعر البيع أقل من سعر التكلفة!').show();
        } else {
            $('#SellingPrice').removeClass('is-invalid');
            $('#priceWarning').text('').hide();
        }
    }

    $('#Quantity, #CostPrice, #SellingPrice').on('input', updateTotals);

    // Validate BatchNumber uniqueness on blur
    $('#BatchNumber').on('blur', function () {
        var batchNumber = $(this).val();
        if (batchNumber) {
            $.get('/StockEntry/ValidateBatchNumber', { batchNumber: batchNumber })
                .done(function (isValid) {
                    if (!isValid) {
                        $('#BatchNumber').addClass('is-invalid');
                        $('#batchWarning').text('رقم الدفعة موجود مسبقاً').show();
                    } else {
                        $('#BatchNumber').removeClass('is-invalid');
                        $('#batchWarning').text('').hide();
                    }
                });
        }
    });

    // Load product variants when the selected product changes
    $('#ProductId').on('change', function () {
        var productId = $(this).val();
        if (productId) {
            $.get('/StockEntry/GetProductVariants', { productId: productId })
                .done(function (variants) {
                    var $variantSelect = $('#ProductVariantId');
                    $variantSelect.empty().append($('<option/>').attr('value', '').text('اختر النوع'));
                    $.each(variants, function (i, variant) {
                        $variantSelect.append($('<option/>').attr('value', variant.value).text(variant.text));
                    });
                });
        }
    });
});
