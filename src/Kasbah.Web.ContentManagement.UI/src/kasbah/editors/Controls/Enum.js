import React from 'react'

class Enum extends React.PureComponent {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    options: React.PropTypes.object,
    id: React.PropTypes.string.isRequired,
    className: React.PropTypes.string
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
