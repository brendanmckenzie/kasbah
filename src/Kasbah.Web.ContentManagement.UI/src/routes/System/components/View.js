import React from 'react'
import { Route, Switch } from 'react-router'
import Navigation from './Navigation'
import Sites from './Sites'

export const View = () => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>System</h1>
      <div className='columns'>
        <div className='column is-2'>
          <Navigation />
        </div>
        <div className='column'>
          <Switch>
            <Route exact path='/system/sites' component={Sites} />
          </Switch>
        </div>
      </div>
    </div>
  </div>
)

export default View
