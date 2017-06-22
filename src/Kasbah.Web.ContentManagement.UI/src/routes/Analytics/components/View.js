import React from 'react'
import PropTypes from 'prop-types'
import Navigation from './Navigation'

export const View = ({ children }) => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>Analytics</h1>

      <div className='columns'>
        <div className='column has-text-centered'>
          <p className='heading'>Active users</p>
          <p className='title'>5</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading' title='In the past 24 hours'>Unique users</p>
          <p className='title'>500</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading'>Page views</p>
          <p className='title'>2,500</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading'>Bounce rate</p>
          <p className='title'>4.2<small>%</small></p>
        </div>
      </div>

      <div className='columns'>
        <div className='column is-2'>
          <Navigation />
        </div>
        <div className='column'>
          {children}
        </div>
      </div>
    </div>
  </div>
)

View.propTypes = {
  children: PropTypes.any
}

export default View
