import React from 'react'
import { Route, Switch } from 'react-router'
import ContentTree from 'components/ContentTree'
import ContentDetail from './ContentDetail'

const View = () => (
  <div className='section'>
    <div className='container'>
      <div className='columns'>
        <div className='column is-2'>
          <Switch>
            <Route exact path='/content' render={props => <ContentTree context='browser' />} />
            <Route exact path='/content/:id'
              render={props => <ContentTree context='browser' selected={props.match.params.id} />} />
          </Switch>
        </div>
        <div className='column'>
          <Route exact path='/content/:id' component={ContentDetail} />
        </div>
      </div>
    </div>
  </div>
)

export default View
