var myMap;
var objectManager;
var heatmap;

// Дождёмся загрузки API и готовности DOM.
ymaps.ready(init);

function init() {
    // Создание экземпляра карты и его привязка к контейнеру с
    // заданным id ("map").
    ymaps.geocode('Томск').then(function (result) {
            myMap = new ymaps.Map('map', {
                // При инициализации карты обязательно нужно указать
                // её центр и коэффициент масштабирования.
                center: result.geoObjects.get(0).geometry.getCoordinates(),
                zoom: 12,
                controls: ['zoomControl', 'rulerControl']
            },
            {
                autoFitToViewport: 'always'
        }),
        objectManager = new ymaps.ObjectManager({
             geoObjectOpenBalloonOnClick: false
        });

        myMap.events.add('boundschange', function (event) {
            if (event.get('newZoom') != event.get('oldZoom')) {
                //heatmap.options.set('radius', 100*myMap.getZoom());
            }
        });
        
        ymaps.modules.require(['Heatmap'], function (Heatmap) {
            heatmap = new Heatmap();
            heatmap.options.set('radius', 100);
            heatmap.options.set('opacity', 0.8);
            heatmap.options.set('intensityOfMidpoint', 1);
            heatmap.options.set('dissipating', true);
        });

        initButtons();

        $.ajax({
            url: "/home/GetAreas"
        }).done(function (data) {            
            objectManager.add(data);
            objectManager.objects.events.add('click', function (e) {
                var id = e.get('objectId');
                $('#centres [value='+id+']').attr("selected", "selected").change();

            });            
            heatmap.setData(data);
        }).error(function(data){
            
        });
    });
}

function initButtons() {
    var heatmapBtn = new ymaps.control.Button({
        data: { content: '<b>Тепловая карта</b>' },
        options: { maxWidth: 180 }
    });

    heatmapBtn.events
        .add('select', function () {
            heatmap.setMap(myMap);
        })
        .add('deselect', function () {
            heatmap.setMap(null);
        });

    var areaNumbersBtn = new ymaps.control.Button({
        data: { content: '<b>Номера</b>' },
        options: {
            maxWidth: 180,
            visible: false,
            position: {
                left: 10,
                top: 40
            }
        }
    });

    areaNumbersBtn.events
        .add('select', function () {
            objectManager.objects.each(function(o) {
                objectManager.objects.setObjectOptions(o.id, {
                    preset: 'islands#circleIcon'
                });
            });
        })
        .add('deselect', function () {
            objectManager.objects.each(function (o) {
                objectManager.objects.setObjectOptions(o.id, {
                    preset: 'islands#circleDotIcon'
                });
            });
        });

    var areaCentres = new ymaps.control.Button({
        data: { content: '<b>Центры зон</b>' },
        options: { maxWidth: 180 }
    });

    areaCentres.events
        .add('select', function () {
            myMap.geoObjects.add(objectManager);
            areaNumbersBtn.options.set('visible', true);
            $('#listBox').toggleClass("hidden");
            $('#map').removeClass("col-lg-12").addClass("col-lg-9");
        })
        .add('deselect', function () {
            myMap.geoObjects.removeAll();
            areaNumbersBtn.options.set('visible', false);
            $('#listBox').toggleClass("hidden");
            $('#map').removeClass("col-lg-9").addClass("col-lg-12");
        });

    $('#centres').bind('change', function() {
        myMap.setCenter(objectManager.objects.getById(this.value).geometry.coordinates);
        objectManager.objects.balloon.open(this.value);
        $('#property_id').val(objectManager.objects.getById(this.value).id);
        $('#property_name').val(objectManager.objects.getById(this.value).properties.name);
        $('#property_location').val(objectManager.objects.getById(this.value).properties.location);
        $('#property_weight').val(objectManager.objects.getById(this.value).properties.weight);

        $('#busstations').empty();
        var list = document.getElementById('busstations');        
        objectManager.objects.getById(this.value).properties.station.forEach(function (item, i, arr) {            
            var li = document.createElement('LI');
            li.innerHTML = item.id + '. ' + item.name;
            list.appendChild(li);
        });
        $('#buildings_count').val(objectManager.objects.getById(this.value).properties.buildings);
        
    });

    myMap.controls.add(heatmapBtn);
    myMap.controls.add(areaCentres);
    myMap.controls.add(areaNumbersBtn);
}

