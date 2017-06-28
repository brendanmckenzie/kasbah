import React from 'react'
import { NavLink } from 'react-router-dom'

export const Navigation = () => (
  <div className='menu'>
    <span className='menu-label'>Sections</span>
    <ul className='menu-list'>
      <li className='menu-item'>
        <NavLink activeClassName='is-active' onlyActiveOnIndex to='/security'>Users</NavLink>
      </li>
    </ul>
  </div>
)

export default Navigation
