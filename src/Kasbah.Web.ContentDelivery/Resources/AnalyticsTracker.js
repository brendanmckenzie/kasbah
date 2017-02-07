(function () {
  if (!window.fetch) {
    // TODO: pull in fetch polyfill
  }

  var AnalyticsTracker = function (existingEvents) {
    this.init(existingEvents)
  }

  AnalyticsTracker.prototype.init = function (existingEvents) {
    if (existingEvents instanceof Array) {
      for (var i = 0; i < existingEvents.length; i++) {
        this.push(existingEvents[i]);
      }
    }
  }

  AnalyticsTracker.prototype.push = function (ev) {
    this.makeRequest('/analytics/track', {
      type: ev.type,
      source: ev.source,
      data: ev.data || {},
      persona: typeof localStorage['kasbah:persona'] === 'undefined' ? null : localStorage['kasbah:persona'],
      session: typeof sessionStorage['kasbah:session'] === 'undefined' ? null : sessionStorage['kasbah:session']
    })
  }

  AnalyticsTracker.prototype.makeRequest = function (endpoint, data) {
    var fetchReq = {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Cookie': document.cookie
      },
      body: data ? JSON.stringify(data) : null,
      credentials: 'same-origin'
    }

    return fetch(endpoint, fetchReq)
  }

  window.analytics = new AnalyticsTracker(window.analytics)

})()
