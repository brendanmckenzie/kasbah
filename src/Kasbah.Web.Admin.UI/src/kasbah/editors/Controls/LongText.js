import React from 'react'

const LongText = ({ input: { value, onChange }, className }) => (
  <textarea className={className} onChange={onChange} value={value} />
)

LongText.propTypes = {
  input: React.PropTypes.object.isRequired,
  className: React.PropTypes.string
}

LongText.alias = 'longText'

export default LongText
