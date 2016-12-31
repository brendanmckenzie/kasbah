import React from 'react'
import ContentTree from './ContentTree'

export const View = ({ children }) => (
  <div className='section'>
    <h1 className='title'>Content</h1>
    <div className='columns'>
      <div className='column is-2'>
        <ContentTree />
      </div>
      <div className='column'>
        {children}
      </div>
    </div>
  </div>
)

View.propTypes = {
  children: React.PropTypes.node
}

export default View
