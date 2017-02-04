(function () {
  if (!window.fetch) {
    // TODO: pull in fetch polyfill
  }

  var AnalyticsTracker = function (existingEvents) {
    this.init(existingEvents);
  }

  AnalyticsTracker.prototype.checkCookie = function () {
    if (document.cookie) {
      var cookies = document.cookie.split(';').map(function (ent) { return ent.trim().split('=') });
      var trackingCookie = cookies.filter(function (ent) { return ent[0] === '__kastrk'; })[0];
      if (trackingCookie) {
        this.trackingCookie = trackingCookie[1];
      }
    }

  }

  AnalyticsTracker.prototype.init = function (existingEvents) {
    this.checkCookie()

    var initReq = {
      persona: localStorage['kasbah:persona'] || null
    }

    this.makeRequest('/analytics/init', initReq).then(function (res) {
      localStorage['kasbah:persona'] = res.persona
      sessionStorage['kasbah:session'] = res.session
    })

    if (existingEvents instanceof Array) {
      for (var i = 0; i < existingEvents.length; i++) {
        this.push(existingEvents[i]);
      }
    }
  };

  AnalyticsTracker.prototype.push = function (ev) {
    this.makeRequest('/analytics/track', {
      type: ev.type,
      source: ev.source,
      data: ev.data || {},
      persona: typeof localStorage['kasbah:persona'] === 'undefined' ? null : localStorage['kasbah:persona'],
      session: typeof sessionStorage['kasbah:session'] === 'undefined' ? null : sessionStorage['kasbah:session']
    })
  };

  AnalyticsTracker.prototype.makeRequest = function (endpoint, data) {
    var fetchReq = {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    }

    return fetch(endpoint, fetchReq)
  };

  window.analytics = new AnalyticsTracker(window.analytics);

})()
