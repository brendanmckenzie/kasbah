import React from 'react'
import PropTypes from 'prop-types'

const Text = ({ input: { value, onChange }, className }) => (
  <input type='text' className={className} onChange={onChange} value={value} />
)

Text.propTypes = {
  input: PropTypes.object.isRequired,
  className: PropTypes.string
}

Text.alias = 'text'

export default Text
