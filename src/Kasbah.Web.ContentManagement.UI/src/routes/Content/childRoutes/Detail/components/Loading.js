import React from 'react'

export const Loading = ({ message }) => (
  <div>
    <div className='message'>
      <div className='message-body'>
        {message || 'Loading...'}
      </div>
    </div>
  </div>
)

Loading.propTypes = {
  message: React.PropTypes.any
}

export default Loading
