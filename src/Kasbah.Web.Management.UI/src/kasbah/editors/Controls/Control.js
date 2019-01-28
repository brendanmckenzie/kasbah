import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form'
import { connect } from 'react-redux'
import _ from 'lodash'
import Nested from './Nested'

class Control extends React.PureComponent {
  static propTypes = {
    input: PropTypes.object.isRequired,
    options: PropTypes.object,
    id: PropTypes.string.isRequired,
    className: PropTypes.string,
  }

  static alias = 'kasbah_web:control'

  state = { selectedControl: '' }

  handleChange = (field) => (ev) => {
    this.setState({
      [field]: ev.target.value,
    })
  }

  handleAddControl = (placeholder) => () => {
    const {
      input: { value, onChange },
    } = this.props

    const newValue = _.merge({}, value, {
      placeholders: {
        [placeholder]: [{ alias: this.state.selectedControl }],
      },
    })

    onChange(newValue)
  }

  get placeholders() {
    const {
      input: { value, name },
      content: {
        components: { list },
      },
    } = this.props

    if (value && value.alias) {
      const component = list.find((ent) => ent.alias === value.alias)
      // console.log(list)
      if (component && component.placeholders.length) {
        return (
          <ul
            style={{
              paddingLeft: 10,
              borderLeftWidth: 5,
              borderLeftColor: '#ccc',
              borderLeftStyle: 'solid',
            }}
          >
            {component.placeholders.map((plc) => (
              <li key={plc.alias}>
                <p>
                  <code>{plc.alias}</code>
                </p>
                <div>
                  <span className="select">
                    <select value={this.state.selectedControl} onChange={this.handleChange('selectedControl')}>
                      <option key={null} value={null}>None</option>
                      {plc.allowedControls.map((c) => (
                        <option value={c} key={c} children={c} />
                      ))}
                    </select>
                  </span>
                  <button type="button" className="button is-small" onClick={this.handleAddControl(plc.alias)}>
                    add control
                  </button>
                </div>
                <div>
                  {value.placeholders &&
                    value.placeholders[plc.alias] &&
                    value.placeholders[plc.alias].map((cnt, idx) => (
                      <React.Fragment key={idx}>
                        <Field
                          component={Control}
                          name={`${name}.placeholders[${plc.alias}][${idx}]`}
                          content={this.props.content}
                        />
                      </React.Fragment>
                    ))}
                </div>
              </li>
            ))}
          </ul>
        )
      }
    }

    return null
  }

  get model() {
    const {
      input: { value, name },
      content: {
        components: { list },
      },
    } = this.props

    if (value && value.alias) {
      const component = list.find((ent) => ent.alias === value.alias)
      if (component) {
        return (
          <ul>
            <li>
              <strong>model</strong>
            </li>
            <li>
              {component.properties && (
                <Field name={`${name}.model`} component={Nested} options={component.properties} />
              )}
              {!component.properties && <span>no model for this component</span>}
            </li>
          </ul>
        )
      }
    }

    return null
  }

  render() {
    const {
      input: { name },
      content: {
        components: { list },
      },
    } = this.props

    return (
      <div className="control-editor">
        <span className="select">
          <Field name={`${name}.alias`} component="select">
            <option value={null}></option>
            {list.map((cmp) => (
              <option key={cmp.alias} value={cmp.alias} children={cmp.alias} />
            ))}
          </Field>
        </span>
        {this.model}
        {this.placeholders}
      </div>
    )
  }
}

const mapStateToProps = (state) => ({
  content: state.content,
})

const mapDispatchToProps = {}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Control)
