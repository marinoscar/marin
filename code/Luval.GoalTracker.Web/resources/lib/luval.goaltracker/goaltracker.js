var goaltracker = {
    init: function () {
        goaltracker.initValidation();
    },
    initValidation: function () {
        $('.needs-validation').each(function (i, el) {

            $(el).on('submit', function (ev) {

                if (!$(el)[0].checkValidity()) {

                    ev.preventDefault();
                    ev.stopPropagation();

                }
                $(el).addClass('was-validated');
            });
        });
    },
    runCharts: function () {
        $('[data-item]').each(function (i, e) {
            goaltracker.createChart(e, null);
        });
    },
    createChart: function (e, f) {
        var id = $(e).data('item');
        var val = $(e).val();
        if (val === null || val === undefined) return;
        var dataArray = val.split(',');
        var ctx = document.getElementById('chart-' + id);
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Weekly', 'Monthly', 'Yearly'],
                datasets: [{
                    data: dataArray,
                    backgroundColor: [
                        'rgb(0, 123, 255)',
                        'rgb(108, 117, 125)',
                        'rgb(40, 167, 69)'
                    ],
                    hoverOffset: 4
                }]
            },
            options: {
                scales: {
                    x: {
                        min: 0,
                        max: 100
                    }
                },
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: false
                    },
                    legend: {
                        display: false
                    }
                }
            }
        });
    }
}