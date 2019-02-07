import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form'
import { connect } from 'react-redux'
import { actions as contentActions } from 'store/appReducers/content'
import Nested from './Nested'

class Control extends React.PureComponent {
  static propTypes = {
    input: PropTypes.object.isRequired,
    options: PropTypes.object,
    className: PropTypes.string,
    content: PropTypes.object.isRequired,
    listComponents: PropTypes.func.isRequired,
  }

  static alias = 'kasbah_web:control'

  state = { selectedControl: '', visibleEditors: {} }

  componentWillMount() {
    if (!this.props.content.components.loaded && !this.props.content.components.loading) {
      this.props.listComponents()
    }
  }

  handleChange = (field) => (ev) => {
    this.setState({
      [field]: ev.target.value,
    })
  }

  handleAddControl = (placeholder) => () => {
    const {
      input: { value, onChange },
    } = this.props

    const newValue = {
      ...value,
      placeholders: {
        [placeholder]: [
          ...((value.placeholders && value.placeholders[placeholder]) || []),
          { alias: this.state.selectedControl },
        ],
      },
    }

    onChange(newValue)
  }

  handleRemoveControl = (placeholder, index) => () => {
    // eslint-disable-next-line no-restricted-globals
    if (!confirm('Are you sure?')) {
      return
    }

    const {
      input: { value, onChange },
    } = this.props

    const newValue = {
      ...value,
      placeholders: {
        [placeholder]: value.placeholders[placeholder].filter((_, idx) => idx !== index),
      },
    }

    onChange(newValue)
  }

  handleMoveDown = (placeholder, index) => () => {
    const move = (arr, from, to) => {
      const actualTo = to === arr.length ? 0 : to
      const clone = [...arr]
      Array.prototype.splice.call(clone, actualTo, 0, Array.prototype.splice.call(clone, from, 1)[0])
      return clone
    }

    const {
      input: { value, onChange },
    } = this.props

    const newValue = {
      ...value,
      placeholders: {
        [placeholder]: move(value.placeholders[placeholder], index, index + 1),
      },
    }

    onChange(newValue)
  }

  handleToggleEditor = (placeholder, index) => () => {
    this.setState((prevState) => ({
      visibleEditors: {
        ...prevState.visibleEditors,
        [`${placeholder}.${index}`]: !prevState.visibleEditors[`${placeholder}.${index}`],
      },
    }))
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
                  <code>placeholder '{plc.alias}'</code>
                </p>
                <div>
                  {value.placeholders &&
                    value.placeholders[plc.alias] &&
                    value.placeholders[plc.alias].map((cnt, idx) => (
                      <React.Fragment key={idx}>
                        <button
                          type="button"
                          className="button is-small"
                          onClick={this.handleToggleEditor(plc.alias, idx)}
                        >
                          {this.state.visibleEditors[`${plc.alias}.${idx}`] ? 'hide' : 'show'} {cnt.alias} editor
                        </button>
                        {this.state.visibleEditors[`${plc.alias}.${idx}`] && (
                          <Field
                            component={Control}
                            name={`${name}.placeholders[${plc.alias}][${idx}]`}
                            content={this.props.content}
                            listComponents={this.props.listComponents}
                          />
                        )}
                        <button
                          type="button"
                          className="button is-warning is-small"
                          onClick={this.handleRemoveControl(plc.alias, idx)}
                        >
                          remove
                        </button>
                        <button type="button" className="button is-small" onClick={this.handleMoveDown(plc.alias, idx)}>
                          move down
                        </button>
                        <hr />
                      </React.Fragment>
                    ))}
                </div>
                <div>
                  <span className="select">
                    <select value={this.state.selectedControl} onChange={this.handleChange('selectedControl')}>
                      <option key={null} value={null}>
                        None
                      </option>
                      {(plc.allowedControls || list.map((a) => a.alias)).map((c) => (
                        <option value={c} key={c} children={c} />
                      ))}
                    </select>
                  </span>
                  <button type="button" className="button is-small" onClick={this.handleAddControl(plc.alias)}>
                    add control
                  </button>
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
            <option value={null} />
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

const mapDispatchToProps = {
  ...contentActions,
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Control)
