import React from 'react'
import PropTypes from 'prop-types'

const LongText = ({ input: { value, onChange }, className }) => (
  <textarea className={className} onChange={onChange} value={value} />
)

LongText.propTypes = {
  input: PropTypes.object.isRequired,
  className: PropTypes.string
}

LongText.alias = 'longText'

export default LongText
