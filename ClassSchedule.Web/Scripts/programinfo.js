﻿function ProgramInfo(data) {
    var self = this;
    if (!data) {
        self.TheoreticalTrainingWeeks = ko.observable('');
        self.ExamSessionWeeks = ko.observable('');
        self.WeeksOfHolidays = ko.observable('');
        self.FinalQualifyingWorkWeeks = ko.observable('');
        self.StudyTrainingWeeks = ko.observable('');
        self.PracticalTrainingWeeks = ko.observable('');
        self.StateExamsWeeks = ko.observable('');
        self.ResearchWorkWeeks = ko.observable('');
    }

    ko.mapping.fromJS(data, {}, self);

    self.highcharts = function (data) {
        //$('#chart-modal').on('show.bs.modal', function () {
        //    $('#container').css('visibility', 'hidden');
        //});
        //$('#chart-modal').on('shown.bs.modal', function () {
        //    $('#container').css('visibility', 'initial');
        //    chart.reflow();
        //});

        $('#chart').highcharts({
            credits: {
                enabled: false
            },
            chart: {
                type: 'columnrange',
                inverted: true
            },
            title: {
                text: ''
            },
            scrollbar: {
                enabled: true
            },
            xAxis: {
                categories: ['Т', 'Э', 'К', 'Н', 'У', 'П', 'Д', 'Г'],
                //labels: {
                //    formatter: function () {
                //        return '$' + this.axis.defaultLabelFormatter.call(this);
                //    }
                //}
            },
            yAxis: {
                type: 'datetime',
                title: {
                    text: ''
                }
            },
            plotOptions: {
                columnrange: {
                    grouping: false
                }
            },
            legend: {
                enabled: false
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.x + ' - ' + this.series.name + '</b><br/>' + Highcharts.dateFormat('%e %B %H:%M', this.point.low) +
                        ' - ' + Highcharts.dateFormat('%B %e %H:%M', this.point.high) + '<br/>';
                }
            },

            series: [{
                name: 'Э',
                pointWidth: 15,
                data: [{
                    x: 4,
                    low: Date.UTC(2013, 07, 03, 0, 0, 0),
                    high: Date.UTC(2013, 07, 03, 4, 0, 0)
                }, {
                    x: 4,
                    low: Date.UTC(2013, 07, 03, 10, 0, 0),
                    high: Date.UTC(2013, 07, 03, 12, 0, 0)
                }, {
                    x: 4,
                    low: Date.UTC(2013, 07, 03, 14, 0, 0),
                    high: Date.UTC(2013, 07, 03, 15, 0, 0)
                }

                ]
            }, {
                name: 'Т',
                pointWidth: 15,
                data: [{
                    x: 1,
                    low: Date.UTC(2013, 07, 03, 4, 0, 0),
                    high: Date.UTC(2013, 07, 03, 10, 0, 0)
                }, {
                    x: 1,
                    low: Date.UTC(2013, 07, 03, 18, 0, 0),
                    high: Date.UTC(2013, 07, 03, 24, 0, 0)
                }]
            }, {
                name: 'У',
                pointWidth: 15,
                data: [{
                    x: 6,
                    low: Date.UTC(2013, 07, 04, 1, 0, 0),
                    high: Date.UTC(2013, 07, 04, 5, 0, 0)
                }, {
                    x: 6,
                    low: Date.UTC(2013, 07, 02, 10, 0, 0),
                    high: Date.UTC(2013, 07, 02, 23, 0, 0)
                }, ]
            }, {
                name: 'Г',
                pointWidth: 15,
                data: [{
                    x: 0,
                    low: Date.UTC(2013, 07, 04, 5, 0, 0),
                    high: Date.UTC(2013, 07, 04, 13, 0, 0)
                }, {
                    x: 0,
                    low: Date.UTC(2013, 07, 02, 2, 0, 0),
                    high: Date.UTC(2013, 07, 02, 10, 0, 0)
                }]
            }]
        });
    };

    return self;
};