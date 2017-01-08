(function () {

    var AnalyticsTracker = function (existingEvents) {
        this.makeRequest = function (endpoint, data, callback) {

        }

        this.init = function () {
            var initReq = {
                persona: localStorage['kasbah:persona']
            }
            var initCallback = function (res) {
                localStorage['kasbah:persona'] = res.persona
                sessionStorage['kasbah:session'] = res.session
            }
            this.makeRequest('init', initReq, initCallback)

            if (typeof existingEvents === 'Array') {
                for (var i = 0; i < existingEvents.length; i++) {
                    this.push(existingEvents[i]);
                }
            }
        }

        this.push = function (ev) {
            this.makeRequest('init', {
                type: ev.type,
                source: ev.source,
                data: ev.data || {},
                persona: localStorage['kasbah:persona'],
                session: sessionStorage['kasbah:session']
            })
        }

        this.init();
    }

    window.analytics = new AnalyticsTracker(window.analytics);

})()