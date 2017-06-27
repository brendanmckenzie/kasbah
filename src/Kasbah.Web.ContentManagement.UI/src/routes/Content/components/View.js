import React from 'react'
import PropTypes from 'prop-types'
import ContentTree from './ContentTree'

const View = ({ children }) => (
  <div className='section'>
    <div className='container'>
      <div className='columns'>
        <div className='column is-2'>
          <ContentTree />
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
