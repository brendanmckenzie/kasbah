import React from 'react'

export const View = () => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>System</h1>

      <div className='columns'>
        <div className='column has-text-centered'>
          <p className='heading'>System start date</p>
          <p className='title'>Sun 8 Jan</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading' title='In the past 24 hours'>Uptime</p>
          <p className='title'>4 days 12 hours</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading'>Requests served</p>
          <p className='title'>1,200,234</p>
        </div>
        <div className='column has-text-centered'>
          <p className='heading'>Requests per second</p>
          <p className='title'>0.24</p>
        </div>
      </div>

      <div className='columns'>
        <div className='column'>
          system logs go here
        </div>
        <div className='column'>
          tables and graphs here
        </div>
      </div>
    </div>
  </div>
)

export default View
