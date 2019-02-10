import React from 'react'
import PropTypes from 'prop-types'

const Boolean = ({ input: { value, onChange }, className }) => (
  <input type='checkbox' className={className} onChange={onChange} value={value} />
)

Boolean.propTypes = {
  input: PropTypes.object.isRequired,
  className: PropTypes.string
}

Boolean.alias = 'boolean'

export default Boolean
