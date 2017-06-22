import React from 'react'
import PropTypes from 'prop-types'

class Enum extends React.PureComponent {
  static propTypes = {
    input: PropTypes.object.isRequired,
    options: PropTypes.object,
    id: PropTypes.string.isRequired,
    className: PropTypes.string
  }

  static alias = 'enum'

  render() {
    const { options, id, input, className } = this.props

    return (<div className='enum-editor'>
      <select id={`${options.enumType}${id}`} name={id}
        className={className} onChange={input.onChange} value={input.value}>
        {options.enum.map(opt =>
            (<option key={`${options.enumType}${opt.value}`} value={opt.value}>{opt.name}</option>)
        )}
      </select>
    </div>)
  }
}

export default Enum
