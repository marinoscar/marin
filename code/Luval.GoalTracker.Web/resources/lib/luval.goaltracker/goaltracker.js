var goaltracker = {
    init: function () {
        return true;
    },
    runCharts: function () {
        var items = ['wp', 'mp', 'yp'];
        for (var i = 0; i < items.length; i++) {
            var f = items[i];
            $('[data-' + f + ']').each(function (i, e) {
                goaltracker.createChart(e, f);
            });
        }
    },
    createChart: function (e, f) {
        var id = $(e).data(f);
        var val = $(e).val();
        if (val === null || val === undefined) return;
        var ctx = document.getElementById(f + '-' + id);
        var myChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Pending', 'Progress'],
                datasets: [{
                    label: 'My Progress',
                    data: [100 - val, val],
                    backgroundColor: [
                        'rgb(255, 99, 132)',
                        'rgb(54, 162, 235)',
                        'rgb(255, 205, 86)'
                    ],
                    hoverOffset: 4
                }]
            },
            options: {
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