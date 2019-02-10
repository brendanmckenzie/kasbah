import React from 'react'
import PropTypes from 'prop-types'

const Boolean = ({ input: { value, onChange } }) => <input type="checkbox" onChange={onChange} value={value} />

Boolean.propTypes = {
  input: PropTypes.object.isRequired,
}

Boolean.alias = 'boolean'

export default Boolean
