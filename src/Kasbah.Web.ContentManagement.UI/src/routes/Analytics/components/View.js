import React from 'react'

export const View = () => (
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
        <div className='column'>
          tables and graphs here
        </div>
        <div className='column'>
          tables and graphs here
        </div>
      </div>
    </div>
  </div>
)

export default View
