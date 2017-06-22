import React from 'react'
import PropTypes from 'prop-types'
import Navigation from './Navigation'

export const View = ({ children }) => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>Security</h1>
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
  children: PropTypes.node
}

export default View
