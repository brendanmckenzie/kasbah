import React from 'react'
import ContentEditor from './ContentEditor'
import SideBar from './SideBar'

export const View = () => (
  <div>
    <h2 className='subtitle'>Website</h2>
    <div className='columns'>
      <div className='column'>
        <ContentEditor />
      </div>
      <div className='column is-3'>
        <SideBar />
      </div>
    </div>
  </div>
)

export default View
