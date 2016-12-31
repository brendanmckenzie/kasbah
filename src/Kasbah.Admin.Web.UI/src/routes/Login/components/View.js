import React from 'react'
import moment from 'moment'

export const View = () => (
  <div className='section'>
    <div className='container'>
      <div className='columns'>
        <div className='column is-4 is-offset-4'>
          <h1 className='title'>Login</h1>
          <form>
            <div className='control'>
              <label>Username</label>
              <input className='input' id='userName' />
            </div>
            <div className='control'>
              <label>Password</label>
              <input className='input' id='password' type='password' />
            </div>
            <div className='control has-text-centered'>
              <button className='button is-primary'>Login</button>
            </div>
          </form>
        </div>
        <div className='column is-2'>
          <h5 className='heading'>System information</h5>
          <p>Kasbah v1.0.0-build-5602</p>
        </div>
      </div>
    </div>
  </div>
)

export default View
