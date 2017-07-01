import React from 'react'
import PropTypes from 'prop-types'
import { NavLink } from 'react-router-dom'

const Breadcrumb = ({ content, id }) => {
  const detail = content.detail[id]

  if (!detail) {
    return null
  }

  const { taxonomy } = detail.node

  return (<ul className='breadcrumb'>
    {taxonomy.aliases.map((ent, index) => (
      <li key={index}>
        {taxonomy.ids[index] === id
          ? (<span>{ent}</span>)
          : (<NavLink to={`/content/${taxonomy.ids[index]}`}>{ent}</NavLink>)}
      </li>
    ))}
  </ul>)
}

Breadcrumb.propTypes = {
  id: PropTypes.string.isRequired,
  content: PropTypes.object.isRequired
}

export default Breadcrumb
