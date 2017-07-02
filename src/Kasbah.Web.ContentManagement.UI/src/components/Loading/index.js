import React from 'react'
import PropTypes from 'prop-types'

const Loading = ({ message }) => (
  <div className='message is-primary'>
    <div className='message-body'>
      {message || 'Loading...'}
    </div>
  </div>
)

Loading.propTypes = {
  message: PropTypes.string
}

export default Loading
