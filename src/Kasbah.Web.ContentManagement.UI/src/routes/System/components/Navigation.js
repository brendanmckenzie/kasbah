import React from 'react'
import { NavLink } from 'react-router-dom'

export const Navigation = () => (
  <div className='menu'>
    <span className='menu-label'>Sections</span>
    <ul className='menu-list'>
      <li className='menu-item'>
        <NavLink activeClassName='is-active' to='/system/sites'>Sites</NavLink>
      </li>
    </ul>
  </div>
)

export default Navigation
