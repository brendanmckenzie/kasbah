import React from 'react'
import PropTypes from 'prop-types'

const Error = ({ message }) => (
  <div className='message is-danger'>
    <div className='message-body'>
      {message || 'An error has occurred'}
    </div>
  </div>
)

Error.propTypes = {
  message: PropTypes.string
}

export default Error
