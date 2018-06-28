import React from 'react'

export const kasbah = window.kasbah

const AnalyticsLanding = () => (<span>hello world</span>)

const moduleDefinition = {
  name: 'Kasbah Analytics',
  routes: [
    { path: '/analytics-test', component: AnalyticsLanding }
  ]
}

kasbah.registerModule(moduleDefinition)
