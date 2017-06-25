import React from 'react'
import { Link } from 'react-router'

export const Navigation = () => (
  <div className='menu'>
    <span className='menu-label'>Sections</span>
    <ul className='menu-list'>
      <li className='menu-item'>
        <Link activeClassName='is-active' onlyActiveOnIndex to='/security'>Users</Link>
      </li>
    </ul>
  </div>
)

export default Navigation
