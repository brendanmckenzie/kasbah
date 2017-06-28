import React from 'react'
import { NavLink } from 'react-router-dom'
import Content from './Content'

export const View = (props) => (
  <div className='section'>
    <div className='container'>
      <div className='tile is-ancestor'>
        <div className='tile is-vertical is-8'>
          <div className='tile'>
            <div className='tile is-parent is-vertical'>
              <article className='tile is-child notification is-danger'>
                <p className='heading'>Content authoring and marketing platform</p>
                <p className='title'>Kasbah</p>
                <p className='subtitle'>v1.0.0.0</p>
              </article>
              <NavLink to='/security' className='tile is-child notification is-warning'>
                <p className='title'>Security</p>
                <p className='subtitle'>Manage user access and roles</p>
              </NavLink>
            </div>
            <div className='tile is-parent'>
              <div className='tile is-child notification is-info' />
            </div>
          </div>
          <div className='tile is-parent'>
            <div className='tile is-light is-child notification' />
          </div>
        </div>
        <div className='tile is-parent'>
          <Content {...props} />
        </div>
      </div>
    </div>
  </div>
)

export default View
